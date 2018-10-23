using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.SerializationState;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.JsonConverters
{
    /// <summary>
    /// Provides functionality to convert a JsonApi resource object into a .NET object
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class ResourceObjectConverter : JsonConverter
    {
        private static readonly Regex DataReadPathRegex = new Regex($@"^$|{PropertyNames.Included}(\[\d+\])?$|{"data"}(\[\d+\])?$");

        private readonly Dictionary<Type, string> typeNames = new Dictionary<Type, string>();

        public override bool CanConvert(Type objectType)
        {
            return TypeInfoShim.GetPropertyFromInhertianceChain(objectType.GetTypeInfo(), "Id") != null;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //we may be starting the deserialization here, if thats the case we need to resolve this object as the root
            if (DocumentRootConverter.TryResolveAsRootData(reader, objectType, serializer, out object obj))
                return obj;

            //read into the 'Data' element
            return ReaderUtil.ReadInto(
                reader as ForkableJsonReader ?? new ForkableJsonReader(reader),
                DataReadPathRegex,
                dataReader =>
            {
                //if they have custom convertors registered, we will respect them
                var customConvertor = serializer.Converters.FirstOrDefault(x => x.CanRead && x.CanConvert(objectType));
                if (customConvertor != null && customConvertor != this)
                    return customConvertor.ReadJson(reader, objectType, existingValue, serializer);

                //if the value has been explicitly set to null then the value of the element is simply null
                if (dataReader.TokenType == JsonToken.Null)
                    return null;

                var serializationData = SerializationData.GetSerializationData(dataReader);

                //if we arent given an existing value check the references to see if we have one in there
                //if we dont have one there then create a new object to populate
                if (existingValue == null)
                {
                    var reference = ReaderUtil.ReadAheadToIdentifyObject(dataReader);

                    if (!serializationData.Included.TryGetValue(reference, out existingValue))
                    {
                        existingValue = CreateObject(objectType, reference.Type, serializer);
                        serializationData.Included.Add(reference, existingValue);
                    }
                    if (existingValue is JObject existingValueJObject)
                    {
                        //sometimes the value in the reference resolver is a JObject. This occurs when we
                        //did not know what type it should be when we first read it (i.e. included was processed
                        //before the item). In these cases we will create a new object and read data from the JObject
                        dataReader = new ForkableJsonReader(existingValueJObject.CreateReader(), dataReader.SerializationDataToken);
                        dataReader.Read(); //JObject readers begin at Not Started
                        existingValue = CreateObject(objectType, reference.Type, serializer);
                        serializationData.Included[reference] = existingValue;
                    }
                }

                //additional check to ensure the object we created is of the correct type
                if (!TypeInfoShim.IsInstanceOf(objectType.GetTypeInfo(), existingValue))
                    throw new JsonSerializationException($"Unable to assign object '{existingValue}' to type '{objectType}'");

                PopulateProperties(serializer, existingValue, dataReader);
                return existingValue;
            });
        }

        protected void PopulateProperties(JsonSerializer serializer, object obj, JsonReader reader)
        {
            JsonObjectContract contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(obj.GetType());

            foreach (var propName in ReaderUtil.IterateProperties(reader))
            {
                var successfullyPopulateProperty = ReaderUtil.TryPopulateProperty(
                    serializer,
                    obj,
                    contract.Properties.GetClosestMatchProperty(propName),
                    reader);

                //flatten out attributes onto the object
                if (!successfullyPopulateProperty && propName == PropertyNames.Attributes)
                {
                    foreach (var innerPropName in ReaderUtil.IterateProperties(reader))
                    {
                        ReaderUtil.TryPopulateProperty(
                           serializer,
                           obj,
                           contract.Properties.GetClosestMatchProperty(innerPropName),
                           reader);
                    }
                }

                //flatten out relationships onto the object
                if (!successfullyPopulateProperty && propName == PropertyNames.Relationships)
                {
                    foreach (var innerPropName in ReaderUtil.IterateProperties(reader))
                    {
                        ReaderUtil.TryPopulateProperty(
                            serializer,
                            obj,
                            contract.Properties.GetClosestMatchProperty(innerPropName),
                            reader);
                    }
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (DocumentRootConverter.TryResolveAsRootData(writer, value, serializer))
            {
                return;
            }

            // if they have custom convertors registered, we will respect them
            JsonConverter customConvertor = null;

            foreach (var x in serializer.Converters)
            {
                if (x.CanWrite && x.CanConvert(value.GetType()))
                {
                    customConvertor = x;
                    break;
                }
            }

            if (customConvertor != null)
            {
                customConvertor.WriteJson(writer, value, serializer);
                return;
            }

            const string relationshipsKey = PropertyNames.Relationships + ".";
            const string dataKey = "." + PropertyNames.Data;

            var writerPath = writer.Path;

            var relationshipsIndex = writerPath.IndexOf(relationshipsKey, StringComparison.Ordinal);

            if (relationshipsIndex > 0 && writerPath.IndexOf(dataKey, relationshipsIndex + relationshipsKey.Length, StringComparison.Ordinal) > 0)
            {
                WriteReferenceObjectJson(writer, value, serializer);
            }
            else
            {
                WriteFullObjectJson(writer, value, serializer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteFullObjectJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueType = value.GetType();
            var contractResolver = serializer.ContractResolver;

            if (!(contractResolver.ResolveContract(valueType) is JsonObjectContract contract
                 && contract.Converter is ResourceObjectConverter))
                throw new JsonApiFormatException(writer.Path,
                    $"Expected to find to find resource object, but found '{value}'",
                    "Resource identifier objects MUST contain 'id' members");

            // prepare to start capturing attributes and relationships

            // default list size to the number of expected properties 
            // to reduce allocations
            List<KeyValuePair<string, object>> relationships = null;

            writer.WriteStartObject();

            string id = null;
            var idProperty = contract.Properties.GetClosestMatchProperty(nameof(PropertyNames.Id));
            if (idProperty != null)
            {
                id = idProperty.ValueProvider.GetValue(value)?.ToString();
                if (id != null)
                {
                    writer.WritePropertyName(PropertyNames.Id, false);
                    writer.WriteValue(id);
                }
            }

            //A resource object MUST contain at least the following top-level members: type
            var typeProperty = contract.Properties.GetClosestMatchProperty(nameof(PropertyNames.Type));

            writer.WritePropertyName(PropertyNames.Type, false);
            var type = typeProperty == null
                ? GetDefaultTypeName(valueType)
                : typeProperty.ValueProvider?.GetValue(value).ToString() ?? GetDefaultTypeName(valueType);
            writer.WriteValue(type);

            void SerializeKnownProperty(string name)
            {
                var metaProperty = contract.Properties.GetClosestMatchProperty(name);

                if (metaProperty == null || metaProperty.Ignored)
                {
                    return;
                }

                var propValue = metaProperty.ValueProvider.GetValue(value);

                if (propValue == null && (metaProperty.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Ignore)
                {
                    return;
                }

                writer.WritePropertyName(name, false);
                serializer.Serialize(writer, propValue);
            }

            SerializeKnownProperty(nameof(PropertyNames.Links));
            SerializeKnownProperty(nameof(PropertyNames.Meta));

            var didWriteAttributes = false;

            for (var index = 0; index < contract.Properties.Count; index++)
            {
                var property = contract.Properties[index];

                switch (property.PropertyName)
                {
                    case PropertyNames.Id:
                    case PropertyNames.Type:
                    case PropertyNames.Links:
                    case PropertyNames.Meta:
                        continue;
                }

                if (property.Ignored)
                {
                    continue;
                }

                var propertyValue = property.ValueProvider.GetValue(value);

                if (propertyValue == null && (property.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Ignore)
                {
                    continue;
                }

                var propertyType = propertyValue?.GetType() ?? property.PropertyType;

                var isRelationship = TryParseAsRelationship(contractResolver.ResolveContract(propertyType), propertyValue, out var relationshipObj);

                if (isRelationship)
                {
                    var collection = relationships ?? (relationships = new List<KeyValuePair<string, object>>());

                    collection.Add(new KeyValuePair<string, object>(property.PropertyName, relationshipObj));

                    continue;
                }

                if (!didWriteAttributes)
                {
                    didWriteAttributes = true;

                    writer.WritePropertyName(PropertyNames.Attributes, false);
                    writer.WriteStartObject();
                }

                writer.WritePropertyName(property.PropertyName, false);

                if (property.MemberConverter != null && property.MemberConverter.CanWrite)
                {
                    property.MemberConverter.WriteJson(writer, propertyValue, serializer);
                }
                else
                {
                    if (propertyValue is string s)
                    {
                        writer.WriteValue(s);
                    }
                    else if (propertyValue is int i)
                    {
                        writer.WriteValue(i);
                    }
                    else
                    {
                        serializer.Serialize(writer, propertyValue);
                    }
                }
            }

            if (didWriteAttributes)
            {
                writer.WriteEndObject();
            }

            // output our relationships
            if (relationships != null)
            {
                writer.WritePropertyName(PropertyNames.Relationships, false);
                writer.WriteStartObject();
                foreach (var relationship in relationships)
                {
                    writer.WritePropertyName(relationship.Key, false);

                    serializer.Serialize(writer, relationship.Value);
                }
                writer.WriteEndObject();
            }

            writer.WriteEndObject();

            if (id == null)
            {
                return;
            }

            // add reference to this type, so others can reference it
            var serializationData = SerializationData.GetSerializationData(writer);
            var reference = new ResourceObjectReference(id, type);
            serializationData.RenderedIncluded.Add(reference);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryParseAsRelationship(JsonContract contract, object value, out object relationshipObj)
        {
            switch (contract.Converter)
            {
                case ResourceObjectConverter _:
                    relationshipObj = Relationship.Create(value);
                    return true;

                case ResourceObjectListConverter _:
                    relationshipObj = Relationship.Create(value as IEnumerable<object>);
                    return true;

                case ResourceRelationshipConverter _:
                    relationshipObj = value ?? contract.DefaultCreator();
                    return true;

                default:
                    relationshipObj = null;
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteReferenceObjectJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var contractResolver = serializer.ContractResolver;

            if (!(contractResolver.ResolveContract(value.GetType()) is JsonObjectContract contract
                && contract.Converter is ResourceObjectConverter))
            {
                throw new JsonApiFormatException(writer.Path,
                    $"Expected to find to find resource object, but found '{value}'",
                    "Resource identifier objects MUST contain 'id' members");
            }

            writer.WriteStartObject();

            //A "resource identifier object" MUST contain type and id members.
            writer.WritePropertyName(PropertyNames.Id, false);
            var idProp = contract.Properties.GetClosestMatchProperty(PropertyNames.Id);
            var idVal = idProp?.ValueProvider?.GetValue(value) ?? string.Empty;
            serializer.Serialize(writer, idVal);

            writer.WritePropertyName(PropertyNames.Type, false);
            var typeProp = contract.Properties.GetClosestMatchProperty(PropertyNames.Type);
            var typeVal = typeProp?.ValueProvider?.GetValue(value) ?? GetDefaultTypeName(value.GetType());
            serializer.Serialize(writer, typeVal);

            //we will only write the object to included if there are properties that have have data
            //that we cant include within the reference
            var willWriteObjectToIncluded = contract.Properties.Any(prop =>
            {
                //ignore id, type, meta and ignored properties
                if (prop.PropertyName == PropertyNames.Id
                    || prop.PropertyName == PropertyNames.Type
                    || prop.PropertyName == PropertyNames.Meta
                    || prop.Ignored)
                    return false;

                //ignore null properties
                var propValue = prop.ValueProvider.GetValue(value);
                return propValue != null || (prop.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Include;
            });

            // typically we would just write the meta in the included. But if we are not going to
            // have something in included we will write the meta inline here
            if (!willWriteObjectToIncluded)
            {
                var metaProp = contract.Properties.GetClosestMatchProperty(PropertyNames.Meta);
                var metaVal = metaProp?.ValueProvider?.GetValue(value);
                if (metaVal != null)
                {
                    writer.WritePropertyName(PropertyNames.Meta, false);
                    serializer.Serialize(writer, metaVal);
                }
            }

            writer.WriteEndObject();

            if (willWriteObjectToIncluded)
            {
                var serializationData = SerializationData.GetSerializationData(writer);
                var reference = new ResourceObjectReference(idVal.ToString(), typeVal.ToString());
                serializationData.Included[reference] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetDefaultTypeName(Type type)
        {
            if (typeNames.TryGetValue(type, out var cachedTypeName))
            {
                return cachedTypeName;
            }

            var typeName = GenerateDefaultTypeName(type);
            typeNames[type] = typeName;
            return typeName;
        }

        /// <summary>
        /// If there is no Type property on the item then this is called to generate a default Type name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string GenerateDefaultTypeName(Type type)
        {
            return type.Name.ToLowerInvariant();
        }

        /// <summary>
        /// Exposes contract to allow overriding object initialisation based on resource type during deserialization.
        /// </summary>
        /// <param name="objectType">Type of the property that the created object will be assigned to</param>
        /// <param name="jsonapiType">Type field specified on on the json api document</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected virtual object CreateObject(Type objectType, string jsonapiType, JsonSerializer serializer)
        {
            var contract = serializer.ContractResolver.ResolveContract(objectType);
            if (contract.DefaultCreator == null)
            {
                var typeInfo = objectType.GetTypeInfo();
                if (typeInfo.IsInterface)
                    throw new JsonSerializationException($"Could not create an instance of type {objectType}. Type is an interface and cannot be instantiated.");
                else if (typeInfo.IsAbstract)
                    throw new JsonSerializationException($"Could not create an instance of type {objectType}. Type is an abstract class and cannot be instantiated.");
                else
                    throw new JsonSerializationException($"Could not create an instance of type {objectType}.");
            }

            return contract.DefaultCreator();
        }
    }
}
