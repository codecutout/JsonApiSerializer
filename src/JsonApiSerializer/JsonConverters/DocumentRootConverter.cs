using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.ReferenceResolvers;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonConverters
{
    internal class DocumentRootConverter : JsonConverter
    {
        public static bool CanConvertStatic(Type objectType)
        {
            return TypeInfoShim.GetInterfaces(objectType.GetTypeInfo())
                .Select(x=>x.GetTypeInfo())
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDocumentRoot<>));
        }

        public override bool CanConvert(Type objectType)
        {
            return CanConvertStatic(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader = new ForkableJsonReader(reader);

            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(objectType);
            var rootObject = contract.DefaultCreator();
            serializer.ReferenceResolver.AddReference(null, IncludedReferenceResolver.RootReference, rootObject);

            var includedConverter = new IncludedConverter();

            foreach (var propName in ReaderUtil.IterateProperties(reader))
            {
                switch (propName)
                {
                    case PropertyNames.Data:

                        var documentRootInterfaceType = TypeInfoShim.GetInterfaces(objectType.GetTypeInfo())
                            .Select(x => x.GetTypeInfo())
                            .FirstOrDefault(x =>
                                x.IsGenericType
                                && x.GetGenericTypeDefinition() == typeof(IDocumentRoot<>));
                        var dataType = documentRootInterfaceType.GenericTypeArguments[0];

                        var dataObj = serializer.Deserialize(reader, dataType);
                        contract.Properties.GetClosestMatchProperty(PropertyNames.Data).ValueProvider.SetValue(rootObject, dataObj);
                        break;
                    case PropertyNames.Included:

                        //if our object has an included property we will do our best to populate it
                        var property = contract.Properties.GetClosestMatchProperty(propName);
                        if(ReaderUtil.CanPopulateProperty(property))
                        {
                            ReaderUtil.TryPopulateProperty(serializer, rootObject, contract.Properties.GetClosestMatchProperty(propName), ((ForkableJsonReader)reader).Fork());
                        }

                        //still need to read our values so they are updated
                        foreach (var obj in ReaderUtil.IterateList(reader))
                        {
                            var includedObject = includedConverter.ReadJson(reader, typeof(object), null, serializer);
                        }
                       
                        break;
                    default:
                        ReaderUtil.TryPopulateProperty(serializer, rootObject, contract.Properties.GetClosestMatchProperty(propName), reader);
                        break;
                }
            }
            return rootObject;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ReferenceResolver.AddReference(null, IncludedReferenceResolver.RootReference, value);

            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());
            writer.WriteStartObject();

            var propertiesOutput = new HashSet<string>();
            foreach(var prop in contract.Properties)
            {
                //we will do includes last, so we we can ensure all the references have been added
                if (prop.PropertyName == PropertyNames.Included)
                    continue;

                //respect the serializers null handling value
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue == null && (prop.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Ignore)
                    continue;

                //A document MAY contain any of these top-level members: jsonapi, links, included
                //We are also allowing everything else they happen to have on the root document
                writer.WritePropertyName(prop.PropertyName);
                serializer.Serialize(writer, propValue);
                propertiesOutput.Add(prop.PropertyName);
            }


            //A document MUST contain one of the following (data, errors, meta)
            //so if we do not have one of them we will output a null data
            if(!propertiesOutput.Contains(PropertyNames.Data)
                && !propertiesOutput.Contains(PropertyNames.Errors) 
                && !propertiesOutput.Contains(PropertyNames.Meta))
            {
                propertiesOutput.Add(PropertyNames.Data);
                writer.WritePropertyName(PropertyNames.Data);
                writer.WriteNull();
            }

            //If a document does not contain a top-level data key, the included member MUST NOT be present
            if (propertiesOutput.Contains(PropertyNames.Data))
            {
                //output the included. If we have a specified included field we will out everything in there
                //and we will also output all the references defined in our reference resolver
                var renderedReferences = (serializer.ReferenceResolver as IncludedReferenceResolver)?.RenderedReferences ?? new HashSet<string>();
                var includedReferences = serializer.ReferenceResolver as IDictionary<string, object> ?? Enumerable.Empty<KeyValuePair<string, object>>();
                includedReferences = includedReferences
                    .Where(x => x.Key != IncludedReferenceResolver.RootReference)
                    .Where(x => !renderedReferences.Contains(x.Key)); //dont output values we have already output
                var includedProperty = contract.Properties.GetClosestMatchProperty(PropertyNames.Included);
                var includedValues = includedProperty?.ValueProvider?.GetValue(value) as IEnumerable<object> ?? Enumerable.Empty<object>();

                //if we have some references we will output them
                if (includedReferences.Any() || includedValues.Any())
                {
                    writer.WritePropertyName(PropertyNames.Included);
                    writer.WriteStartArray();

                    foreach (var includedValue in includedValues)
                    {
                        serializer.Serialize(writer, includedValue);
                    }

                    //I know we can alter the OrderedDictionary while enumerating it, otherwise this would error
                    foreach (var includedReference in includedReferences)
                        serializer.Serialize(writer, includedReference.Value);

                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }

        internal static bool TryResolveAsRootError(JsonReader reader, Type objectType, JsonSerializer serializer, out IEnumerable<IError> obj)
        {
            //if we already have a root object then we dont need to resolve the root object
            if (serializer.ReferenceResolver.ResolveReference(null, IncludedReferenceResolver.RootReference) != null)
            {
                obj = null;
                return false;
            }

            //determine the error class type. The type passed in could be an array or an object
            //so we need to determine the error type for both
            Type errorElementType;
            if (!ListUtil.IsList(objectType, out errorElementType))
                errorElementType = objectType;

            //we do not have a root object, so this is probably the entry point, so we will resolve
            //a document root and return the data object
            var documentRootType = typeof(MinimalDocumentRoot<,>).MakeGenericType(typeof(object), errorElementType);
            var objContract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(documentRootType);
            var dataProp = objContract.Properties.GetClosestMatchProperty("errors");

            var root = serializer.Deserialize(reader, documentRootType);
            obj = (IEnumerable<IError>)dataProp.ValueProvider.GetValue(root);
            return true;
        }

        internal static bool TryResolveAsRootError(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //if we already have a root object then we dont need to resolve the root object
            if (serializer.ReferenceResolver.ResolveReference(null, IncludedReferenceResolver.RootReference) != null)
            {
                return false;
            }

            //coerce single element into a list
            if (value is IError)
                value = new List<IError>() { (IError)value };

            //we do not have a root object, so this is probably the entry point, so we will resolve
            //it as a document root
            var documentRootType = typeof(MinimalDocumentRoot<,>).MakeGenericType(typeof(object), typeof(IError));
            var objContract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(documentRootType);
            var rootObj = objContract.DefaultCreator();

            

            //set the data property to be our current object
            var dataProp = objContract.Properties.GetClosestMatchProperty("errors");
            dataProp.ValueProvider.SetValue(rootObj, value);

            serializer.Serialize(writer, rootObj);
            return true;
        }

        internal static bool TryResolveAsRootData(JsonReader reader, Type objectType, JsonSerializer serializer, out object obj)
        {
            //if we already have a root object then we dont need to resolve the root object
            if (serializer.ReferenceResolver.ResolveReference(null, IncludedReferenceResolver.RootReference) != null)
            {
                obj = null;
                return false;
            }

            //we do not have a root object, so this is probably the entry point, so we will resolve
            //a document root and return the data object
            var documentRootType = typeof(MinimalDocumentRoot<,>).MakeGenericType(objectType, typeof(Error));
            var objContract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(documentRootType);
            var dataProp = objContract.Properties.GetClosestMatchProperty("data");

            var root = serializer.Deserialize(reader, documentRootType);
            obj = dataProp.ValueProvider.GetValue(root);
            return true;
        }

        internal static bool TryResolveAsRootData(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //if we already have a root object then we dont need to resolve the root object
            if (serializer.ReferenceResolver.ResolveReference(null, IncludedReferenceResolver.RootReference) != null)
            {
                return false;
            }

            //we do not have a root object, so this is probably the entry point, so we will resolve
            //it as a document root
            var documentRootType = typeof(MinimalDocumentRoot<,>).MakeGenericType(value.GetType(), typeof(Error));
            var objContract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(documentRootType);
            var rootObj = objContract.DefaultCreator();

            //set the data property to be our current object
            var dataProp = objContract.Properties.GetClosestMatchProperty("data");
            dataProp.ValueProvider.SetValue(rootObj, value);

            serializer.Serialize(writer, rootObj);
            return true;
        }

        private class MinimalDocumentRoot<TData, TError> : IDocumentRoot<TData> where TError : IError
        {
            public TData Data { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public IEnumerable<TError> Errors { get; set; }
        }

    }

}
