using System;
using System.Collections.Generic;
using System.Linq;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json;

namespace JsonApiSerializer.Test.TestUtils
{
    /// <summary>
    /// Provides functionality to convert a JsonApi resoruce object into a .NET object
    /// of either the declared type or as a subclass of the declared
    /// </summary>
    /// <typeparam name="T">The type of member this convertor should be used for</typeparam>
    /// <seealso cref="JsonApiSerializer.JsonConverters.ResourceObjectConverter" />
    public class SubclassResourceObjectConverter<T> : ResourceObjectConverter
    {
        private readonly IReadOnlyDictionary<string, Type> jsonapiTypeToClass;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubclassResourceObjectConverter{T}"/> class.
        /// This constructor is primarily intended for creation through the `[JsonConverterAttribute]
        /// </summary>
        /// <param name="jsonapiType">Type of the jsonapi.</param>
        /// <param name="clazz">The class.</param>
        public SubclassResourceObjectConverter(string jsonapiType, Type clazz)
            : this(new object[][] { new object[] { jsonapiType, clazz } })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubclassResourceObjectConverter{T}"/> class.
        /// This constructor is primarily intended for creation through the `[JsonConverterAttribute]
        /// </summary>
        /// <param name="jsonapiTypeClassPair1">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair2">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        public SubclassResourceObjectConverter(
            object[] jsonapiTypeClassPair1, 
            object[] jsonapiTypeClassPair2)
            : this(new object[][] {
                jsonapiTypeClassPair1,
                jsonapiTypeClassPair2 })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubclassResourceObjectConverter{T}"/> class.
        /// This constructor is primarily intended for creation through the `[JsonConverterAttribute]
        /// </summary>
        /// <param name="jsonapiTypeClassPair1">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair2">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair3">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        public SubclassResourceObjectConverter(
            object[] jsonapiTypeClassPair1, 
            object[] jsonapiTypeClassPair2, 
            object[] jsonapiTypeClassPair3)
            : this(new object[][] {
                jsonapiTypeClassPair1,
                jsonapiTypeClassPair2,
                jsonapiTypeClassPair3 })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubclassResourceObjectConverter{T}"/> class.
        /// This constructor is primarily intended for creation through the `[JsonConverterAttribute]
        /// </summary>
        /// <param name="jsonapiTypeClassPair1">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair2">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair3">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair4">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        public SubclassResourceObjectConverter(
            object[] jsonapiTypeClassPair1,
            object[] jsonapiTypeClassPair2,
            object[] jsonapiTypeClassPair3,
            object[] jsonapiTypeClassPair4)
            : this(new object[][] {
                jsonapiTypeClassPair1,
                jsonapiTypeClassPair2,
                jsonapiTypeClassPair3,
                jsonapiTypeClassPair4 })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubclassResourceObjectConverter{T}"/> class.
        /// This constructor is primarily intended for creation through the `[JsonConverterAttribute]
        /// </summary>
        /// <param name="jsonapiTypeClassPair1">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair2">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair3">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair4">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair5">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        public SubclassResourceObjectConverter(
            object[] jsonapiTypeClassPair1,
            object[] jsonapiTypeClassPair2,
            object[] jsonapiTypeClassPair3,
            object[] jsonapiTypeClassPair4,
            object[] jsonapiTypeClassPair5)
            : this(new object[][] {
                jsonapiTypeClassPair1,
                jsonapiTypeClassPair2,
                jsonapiTypeClassPair3,
                jsonapiTypeClassPair4,
                jsonapiTypeClassPair5 })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubclassResourceObjectConverter{T}"/> class.
        /// This constructor is primarily intended for creation through the `[JsonConverterAttribute]
        /// </summary>
        /// <param name="jsonapiTypeClassPair1">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair2">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair3">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair4">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair5">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        /// <param name="jsonapiTypeClassPair6">array where the first element is the jsonapi type name as a string and the second element is the Type value to instantiate.</param>
        public SubclassResourceObjectConverter(
            object[] jsonapiTypeClassPair1,
            object[] jsonapiTypeClassPair2,
            object[] jsonapiTypeClassPair3,
            object[] jsonapiTypeClassPair4,
            object[] jsonapiTypeClassPair5,
            object[] jsonapiTypeClassPair6)
            : this(new object[][] {
                jsonapiTypeClassPair1,
                jsonapiTypeClassPair2,
                jsonapiTypeClassPair3,
                jsonapiTypeClassPair4,
                jsonapiTypeClassPair5,
                jsonapiTypeClassPair6 })
        { }


        /// <summary>
        /// Initializes a new instance of the <see cref="SubclassResourceObjectConverter{T}"/> class.
        /// This constructor is primarily intended for creation through the `[JsonConverterAttribute]
        /// </summary>
        /// <param name="jsonapiTypeClassPairs">A list of arrays where the first element is the jsonapi type name as a string
        /// and the second element is the Type value to instantiate</param>
        public SubclassResourceObjectConverter(params object[][] jsonapiTypeClassPairs)
            : this(jsonapiTypeClassPairs.OfType<object[]>().ToDictionary(arr => (string)(arr[0]), arr => (Type)arr[1]))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubclassResourceObjectConverter{T}"/> class.
        /// </summary>
        /// <param name="jsonapiTypeToClass">A mapping between the jsonapi type and the .NET class Type</param>
        /// <exception cref="Exception"></exception>
        public SubclassResourceObjectConverter(IDictionary<string, Type> jsonapiTypeToClass)
        {
            this.jsonapiTypeToClass = new Dictionary<string, Type>(jsonapiTypeToClass);

            //add some extra checks to ensure the provided types can actaully be used on fields of type T
            var invalidTypes = this.jsonapiTypeToClass.Values
                .Where(x => !typeof(T).IsAssignableFrom(x))
                .Select(x => x.ToString());
            if (invalidTypes.Any())
            {
                throw new Exception($"Type '{string.Join("', '", invalidTypes)}' are not assignable to member of type {typeof(T).ToString()}");
            }
        }
        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
        }

        protected override object CreateObject(Type objectType, string jsonapiType, JsonSerializer serializer)
        {
            if (jsonapiTypeToClass.TryGetValue(jsonapiType, out Type clazz))
            {
                var contract = serializer.ContractResolver.ResolveContract(clazz);
                return contract.DefaultCreator();
            }
            else
            {
                return base.CreateObject(objectType, jsonapiType, serializer);
            }
        }

        public override bool CanWrite => false;
    }
}
