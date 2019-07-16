using JsonApiSerializer.ContractResolvers;
using JsonApiSerializer.ContractResolvers.Contracts;
using JsonApiSerializer.Exceptions;
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
using System.Text.RegularExpressions;

namespace JsonApiSerializer.JsonConverters
{
    internal class ResourceIdentifierConverter : JsonConverter
    {
        private readonly Func<Type, bool> isResourceObject;


        public ResourceIdentifierConverter(Func<Type, bool> isResourceObject)
        {
            this.isResourceObject = isResourceObject;
        }

        public override bool CanConvert(Type objectType)
        {
            return isResourceObject(objectType)
                || IsArrayOf(objectType, isResourceObject)
                || IsExplicitResourceIdentifier(objectType)
                || IsArrayOf(objectType, IsExplicitResourceIdentifier);
        }

        private bool IsArrayOf(Type type , Func<Type, bool> elementTypeCheck)
        {
            return ListUtil.IsList(type, out Type elementType) 
                && elementTypeCheck(elementType);
        }

        internal bool IsExplicitResourceIdentifier(Type type)
        {
            return TypeInfoShim.GetInterfaces(type.GetTypeInfo())
                .Select(x => x.GetTypeInfo())
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IResourceIdentifier<>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueType = value.GetType();
            var valueContract = serializer.ContractResolver.ResolveContract(valueType);
            switch (valueContract)
            {
                case ResourceObjectContract roc:
                    WriteResourceObjectJson(writer, value, serializer);
                    break;
                case ResourceIdentifierContract ric:
                    WriteExplicitIdentifierJson(writer, value, serializer);
                    break;
                case JsonArrayContract jac:
                    writer.WriteStartArray();
                    var enumerable = value as IEnumerable<object> ?? Enumerable.Empty<object>();
                    foreach (var dataElement in enumerable)
                    {
                        if (dataElement == null)
                            continue;
                        WriteJson(writer, dataElement, serializer);
                    }
                    writer.WriteEndArray();
                    break;
                default:
                    throw new JsonApiFormatException(
                       writer.Path,
                       $"Expected to find a resource identifier or resource object, but found '{value}'",
                       "Resource indentifier objects MUST contain 'id' members");
            }
           
        }

        private void WriteResourceObjectJson(JsonWriter writer, object resourceObject, JsonSerializer serializer)
        {
            if (resourceObject == null)
            {
                writer.WriteNull();
                return;
            }

            var serializationData = SerializationData.GetSerializationData(writer);

            var resourceObjectType = resourceObject.GetType();
            var resourceObjectContract = (ResourceObjectContract)serializer.ContractResolver.ResolveContract(resourceObjectType);

            writer.WriteStartObject();

            //A "resource identifier object" MUST contain type and id members.
            //serialize id
            WriterUtil.ShouldWriteStringProperty(writer, resourceObject, resourceObjectContract.IdProperty, serializer, out string id);
            writer.WritePropertyName(PropertyNames.Id);
            writer.WriteValue(id);

            //serialize type. Will always out put a type
            WriterUtil.ShouldWriteStringProperty(writer, resourceObject, resourceObjectContract.TypeProperty, serializer, out string type);
            type = type ?? WriterUtil.CalculateDefaultJsonApiType(resourceObject, serializationData, serializer);
            writer.WritePropertyName(PropertyNames.Type);
            writer.WriteValue(type);

            //we will only write the object to included if there are properties that have have data
            //that we cant include within the reference
            var willWriteObjectToIncluded = WriterUtil.ShouldWriteProperty(resourceObject, resourceObjectContract.LinksProperty, serializer, out object _);
            for (var i = 0; i < resourceObjectContract.Attributes.Length && !willWriteObjectToIncluded; i++)
            {
                willWriteObjectToIncluded = WriterUtil.ShouldWriteProperty(resourceObject, resourceObjectContract.Attributes[i], serializer, out object _);
            }
            for (var i = 0; i < resourceObjectContract.Relationships.Length && !willWriteObjectToIncluded; i++)
            {
                willWriteObjectToIncluded = WriterUtil.ShouldWriteProperty(resourceObject, resourceObjectContract.Relationships[i], serializer, out object _);
            }

            // typically we would just write the meta in the included. But if we are not going to
            // have something in included we will write the meta inline here
            if (!willWriteObjectToIncluded && WriterUtil.ShouldWriteProperty(resourceObject, resourceObjectContract.MetaProperty, serializer, out object metaVal))
            {
                writer.WritePropertyName(PropertyNames.Meta);
                serializer.Serialize(writer, metaVal);
            }

            writer.WriteEndObject();

            if (willWriteObjectToIncluded)
            {
                var reference = new ResourceObjectReference(id, type);
                serializationData.Included[reference] = resourceObject;
            }
        }

        private void WriteExplicitIdentifierJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var serializationData = SerializationData.GetSerializationData(writer);

            var valueType = value.GetType();
            var resourceIdentifierContract = (ResourceIdentifierContract)serializer.ContractResolver.ResolveContract(valueType);
            var resourceObject = resourceIdentifierContract.ResourceObjectProperty.ValueProvider.GetValue(value);
          
            if (resourceObject == null)
            {
                writer.WriteNull();
                return;
            }

            var resourceObjectType = resourceObject.GetType();
            var resourceObjectContract = (ResourceObjectContract)serializer.ContractResolver.ResolveContract(resourceObjectType);

            writer.WriteStartObject();

            //A "resource identifier object" MUST contain type and id members.
            //serialize id
            WriterUtil.ShouldWriteStringProperty(writer, resourceObject, resourceObjectContract.IdProperty, serializer, out string id);
            writer.WritePropertyName(PropertyNames.Id);
            writer.WriteValue(id);

            //serialize type. Will always out put a type
            WriterUtil.ShouldWriteStringProperty(writer, resourceObject, resourceObjectContract.TypeProperty, serializer, out string type);
            type = type ?? WriterUtil.CalculateDefaultJsonApiType(resourceObject, serializationData, serializer);
            writer.WritePropertyName(PropertyNames.Type);
            writer.WriteValue(type);

            for (var i=0; i < resourceIdentifierContract.Properties.Count; i++)
            {
                var resourceIdentifierProp = resourceIdentifierContract.Properties[i];
                if (resourceIdentifierProp == resourceIdentifierContract.ResourceObjectProperty)
                    continue;
                switch (resourceIdentifierProp.PropertyName)
                {
                    case PropertyNames.Id:
                    case PropertyNames.Type:
                        break;
                    default:
                        if(WriterUtil.ShouldWriteProperty(value, resourceIdentifierProp, serializer, out object propValue))
                        {
                            writer.WritePropertyName(resourceIdentifierProp.PropertyName);
                            serializer.Serialize(writer, propValue);
                        }
                        break;
               }
            }

            writer.WriteEndObject();

            var reference = new ResourceObjectReference(id, type);
            serializationData.Included[reference] = resourceObject;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var serializationData = SerializationData.GetSerializationData(reader);
            var forkableReader = reader as ForkableJsonReader ?? new ForkableJsonReader(reader);
            var jsonApiContractResolver = (JsonApiContractResolver)serializer.ContractResolver;
            var contract = jsonApiContractResolver.ResolveContract(objectType);

            switch (contract)
            {
                case ResourceObjectContract roc:
                    return ReadJsonAsResourceObject(forkableReader, objectType, serializer);
                case ResourceIdentifierContract ric:
                    return ReadJsonAsExplicitResourceIdentifier(forkableReader, objectType, serializer);
                case JsonArrayContract jac when forkableReader.TokenType == JsonToken.StartArray:
                    var list = new List<object>();
                    foreach (var item in ReaderUtil.IterateList(forkableReader))
                        list.Add(ReadJson(forkableReader, jac.CollectionItemType, null, serializer));
                    return ListUtil.CreateList(objectType, list);
                default:
                    return serializer.Deserialize(reader, objectType);
                    throw new JsonApiFormatException(
                       forkableReader.FullPath,
                       $"Expected to find a resource identifier or resource object, but found '{objectType}'",
                       "Resource indentifier objects MUST contain 'id' members");
            }
        }

        private object ReadJsonAsExplicitResourceIdentifier(ForkableJsonReader reader, Type objectType, JsonSerializer serializer)
        {
            if (ReaderUtil.TryUseCustomConvertor(reader, objectType, null, serializer, this, out object customConvertedValue))
                return customConvertedValue;

            // if the value has been explicitly set to null then the value of the element is simply null
            if (reader.TokenType == JsonToken.Null)
                return null;
            
            var serializationData = SerializationData.GetSerializationData(reader);
            var jsonApiContractResolver = (JsonApiContractResolver)serializer.ContractResolver;
            var resourceIdentifierContract = (ResourceIdentifierContract)jsonApiContractResolver.ResolveContract(objectType);
            var resourceIdentifier = resourceIdentifierContract.DefaultCreator();

            var reference = ReaderUtil.ReadAheadToIdentifyObject(reader);

            var explicitResourceIdentifierReader = reader.Fork();
            foreach (var innerPropName in ReaderUtil.IterateProperties(explicitResourceIdentifierReader))
            {
                ReaderUtil.TryPopulateProperty(
                  serializer,
                  resourceIdentifier,
                  resourceIdentifierContract.Properties.GetClosestMatchProperty(innerPropName),
                  explicitResourceIdentifierReader);
            }

            var resourceObject = ReadJsonAsResourceObject(
                       reader,
                       resourceIdentifierContract.ResourceObjectProperty.PropertyType,
                       serializer);

            //we will only set the resource object if we have rendered the included
            //value somehwere, if we have not it means the value was actaully provided
            var valueProvider = resourceIdentifierContract.ResourceObjectProperty.ValueProvider;
            serializationData.PostProcessingActions.Add(() =>
            {
                if (serializationData.RenderedIncluded.Contains(reference))
                    valueProvider.SetValue(resourceIdentifier, resourceObject);
            });

            return resourceIdentifier;
        }

        private object ReadJsonAsResourceObject(ForkableJsonReader reader, Type objectType, JsonSerializer serializer)
        {
            // if the value has been explicitly set to null then the value of the element is simply null
            if (reader.TokenType == JsonToken.Null)
                return null;

            var serializationData = SerializationData.GetSerializationData(reader);
            var jsonApiContractResolver = (JsonApiContractResolver)serializer.ContractResolver;

            var reference = ReaderUtil.ReadAheadToIdentifyObject(reader);

            if (serializationData.Included.TryGetValue(reference, out object resourceObject))
            {
                if (resourceObject is JObject resoruceObjectJObject)
                {
                    // sometimes the value in the reference resolver is a JObject. This occurs when we
                    // did not know what type it should be when we first read it (i.e. included was processed
                    // before the item). In these cases we now know what type it should be so will read it
                    // as such
                    var resourceObjectReader = new ForkableJsonReader(resoruceObjectJObject.CreateReader(), reader.SerializationDataToken);
                    resourceObjectReader.Read(); //JObject readers begin at Not Started
                    resourceObject = jsonApiContractResolver.ResourceObjectConverter.ReadJson(
                        resourceObjectReader, 
                        objectType, 
                        null, 
                        serializer);
                }

                //push the reader to the end, we dont need anything else out of the reference
                ReaderUtil.ReadUntilEnd(reader, reader.Path);
            }
            else
            {
                var contract = (JsonObjectContract)jsonApiContractResolver.ResolveContract(objectType);

                resourceObject = ReaderUtil.CreateObject(serializationData, objectType, reference.Type, serializer);

                // for placeholders we will just read the top level properties
                // it is unlikely to have attributes/relationships present
                foreach (var propName in ReaderUtil.IterateProperties(reader))
                {
                    var successfullyPopulateProperty = ReaderUtil.TryPopulateProperty(
                        serializer,
                        resourceObject,
                        contract.Properties.GetClosestMatchProperty(propName),
                        reader);
                }

                serializationData.Included[reference] = resourceObject;
            }

            if (!TypeInfoShim.IsInstanceOf(objectType.GetTypeInfo(), resourceObject))
                throw new JsonSerializationException($"Unable to assign object '{resourceObject}' to type '{objectType}' at path {reader.FullPath}");

            return resourceObject;
        }
    }
}
