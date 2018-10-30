using JsonApiSerializer.ContractResolvers;
using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.JsonConverters;
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

        public override bool CanConvert(Type objectType)
        {
            return TypeInfoShim.GetPropertyFromInhertianceChain(objectType.GetTypeInfo(), "Id") != null;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //we may be starting the deserialization here, if thats the case we need to resolve this object as the root
            var serializationData = SerializationData.GetSerializationData(reader);
            if (!serializationData.HasProcessedDocumentRoot)
                return DocumentRootConverter.ResolveAsRootData(reader, objectType, serializer);

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
            var serializationData = SerializationData.GetSerializationData(writer);
            if (!serializationData.HasProcessedDocumentRoot)
            {
                //treat this value as a document root
                DocumentRootConverter.ResolveAsRootData(writer, value, serializer);
                return;
            }

            // if they have custom convertors registered, we will respect them
            for (var index = 0; index < serializer.Converters.Count; index++)
            {
                var converter = serializer.Converters[index];

                if (converter.CanWrite && converter.CanConvert(value.GetType()))
                {
                    converter.WriteJson(writer, value, serializer);
                    return;
                }
            }

            // if we are already processing a resource object write out a reference,
            // otherwise write out the full object
            if (serializationData.ResourceObjectStack.Count > 0)
            {
                WriteReferenceObjectJson(writer, value, serializer);
            }
            else
            {
                serializationData.ResourceObjectStack.Push(value);
                try
                {
                    WriteFullObjectJson(writer, value, serializer);
                }
                finally
                {
                    serializationData.ResourceObjectStack.Pop();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteFullObjectJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueType = value.GetType();

            if(!(serializer.ContractResolver.ResolveContract(valueType) is ResourceObjectContract metadata))
                throw new JsonApiFormatException(
                      writer.Path,
                      $"Expected to find to find resource object, but found '{value}'",
                      "Resource indentifier objects MUST contain 'id' members");
           

            writer.WriteStartObject();

            //serialize id
            if (ShouldWriteProperty(value, metadata.IdProperty, serializer, out string id))
            {
                writer.WritePropertyName(PropertyNames.Id);
                writer.WriteValue(id);
            }

            //serialize type. Will always out put a type
            ShouldWriteProperty<string>(value, metadata.TypeProperty, serializer, out string type);
            type = type ?? metadata.DefaultType;
            writer.WritePropertyName(PropertyNames.Type);
            writer.WriteValue(type);

            //serialize links
            if (ShouldWriteProperty(value, metadata.LinksProperty, serializer, out object links))
            {
                writer.WritePropertyName(PropertyNames.Links);
                serializer.Serialize(writer, links);
            }
            
            //serialize meta
            if (ShouldWriteProperty(value, metadata.MetaProperty, serializer, out object meta))
            {
                writer.WritePropertyName(PropertyNames.Meta);
                serializer.Serialize(writer, meta);
            }


            //serialize attributes
            var startedAttributeSection = false;
            for(var i=0;i< metadata.Attributes.Length; i++)
            {
                var attributeProperty = metadata.Attributes[i];
                if (ShouldWriteProperty(value, attributeProperty, serializer, out object attributeValue))
                {
                    if (!startedAttributeSection)
                    {
                        startedAttributeSection = true;
                        writer.WritePropertyName(PropertyNames.Attributes);
                        writer.WriteStartObject();
                    }
                    writer.WritePropertyName(attributeProperty.PropertyName);
                    if (attributeProperty.MemberConverter?.CanWrite == true)
                    {
                        attributeProperty.MemberConverter.WriteJson(writer, attributeValue, serializer);
                    }
                    else if (attributeValue is string attributeString)
                    {
                        writer.WriteValue(attributeString);
                    }
                    else if (attributeValue is bool attributeBool)
                    {
                        writer.WriteValue(attributeBool);
                    }
                    else if (attributeValue is int attributeInt)
                    {
                        writer.WriteValue(attributeValue);
                    }
                    else
                    {
                        serializer.Serialize(writer, attributeValue);
                    }
                }
            }
            if(startedAttributeSection)
                writer.WriteEndObject();

            //serialize relationships
            var startedRelationshipSection = false;
            for (var i = 0; i < metadata.RelationshipTransformations.Length; i++)
            {
                var relationshipProperty = metadata.RelationshipTransformations[i].Key;
                var relationshipTransformation = metadata.RelationshipTransformations[i].Value;
                if (ShouldWriteProperty(value, relationshipProperty, serializer, out object relationshipValue))
                {
                    if (!startedRelationshipSection)
                    {
                        startedRelationshipSection = true;
                        writer.WritePropertyName(PropertyNames.Relationships);
                        writer.WriteStartObject();
                    }
                    writer.WritePropertyName(relationshipProperty.PropertyName);
                    var relationshipObject = relationshipTransformation(relationshipValue);
                    serializer.Serialize(writer, relationshipObject);
                }
            }
            if (startedRelationshipSection)
                writer.WriteEndObject();
            
            writer.WriteEndObject();

            //add reference to this type, so others can reference it
            if (id != null) {
                var serializationData = SerializationData.GetSerializationData(writer);
                var reference = new ResourceObjectReference(id, type);
                serializationData.RenderedIncluded.Add(reference);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteReferenceObjectJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueType = value.GetType();

            if (!(serializer.ContractResolver.ResolveContract(valueType) is ResourceObjectContract metadata))
                throw new JsonApiFormatException(
                      writer.Path,
                      $"Expected to find to find resource object, but found '{value}'",
                      "Resource indentifier objects MUST contain 'id' members");

            writer.WriteStartObject();

            //A "resource identifier object" MUST contain type and id members.
            //serialize id
            ShouldWriteProperty(value, metadata.IdProperty, serializer, out string id);
            writer.WritePropertyName(PropertyNames.Id);
            writer.WriteValue(id);
            
            //serialize type. Will always out put a type
            ShouldWriteProperty(value, metadata.TypeProperty, serializer, out string type);
            type = type ?? metadata.DefaultType;
            writer.WritePropertyName(PropertyNames.Type);
            writer.WriteValue(type);

            //we will only write the object to included if there are properties that have have data
            //that we cant include within the reference
            var willWriteObjectToIncluded = false;
            willWriteObjectToIncluded = ShouldWriteProperty(value, metadata.LinksProperty, serializer, out object _);
            for(var i = 0; i < metadata.Attributes.Length && !willWriteObjectToIncluded; i++)
            {
                willWriteObjectToIncluded = ShouldWriteProperty(value, metadata.Attributes[i], serializer, out object _);
            }
            for (var i = 0; i < metadata.RelationshipTransformations.Length && !willWriteObjectToIncluded; i++)
            {
                willWriteObjectToIncluded = ShouldWriteProperty(value, metadata.RelationshipTransformations[i].Key, serializer, out object _);
            }

            // typically we would just write the meta in the included. But if we are not going to
            // have something in included we will write the meta inline here
            if (!willWriteObjectToIncluded && ShouldWriteProperty(value, metadata.MetaProperty, serializer, out object metaVal))
            {
                writer.WritePropertyName(PropertyNames.Meta);
                serializer.Serialize(writer, metaVal);
            }

            writer.WriteEndObject();

            if (willWriteObjectToIncluded)
            {
                var serializationData = SerializationData.GetSerializationData(writer);
                var reference = new ResourceObjectReference(id, type);
                serializationData.Included[reference] = value;
            }
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

        internal virtual string GenerateDefaultTypeNameInternal(Type type)
        {
            return GenerateDefaultTypeName(type);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ShouldWriteProperty<T>(object value, JsonProperty prop, JsonSerializer serializer, out T propValue)
        {
            if (prop == null)
            {
                propValue = default(T);
                return false;
            }
            propValue = (T)prop.ValueProvider.GetValue(value);
            return propValue != null || (prop.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Include;
        }
    }
}
