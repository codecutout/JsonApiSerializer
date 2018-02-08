using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.Util;
using JsonApiSerializer.Util.JsonApiConverter.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.SerializationState
{
    internal class SerializationData
    {
        private static ConditionalWeakTable<object, SerializationData> _perSerializationData = new ConditionalWeakTable<object, SerializationData>();

        public static SerializationData GetSerializationData(JsonWriter writer)
        {
            object token = writer;
            if (writer is JsonWriterCapture jsonWriterCapture)
                token = jsonWriterCapture.SerializationDataToken;
            return _perSerializationData.GetOrCreateValue(token);
        }

        public static SerializationData GetSerializationData(JsonReader reader)
        {
            object token = reader;
            if (reader is ForkableJsonReader forkableReader)
                token = forkableReader.SerializationDataToken;
            return _perSerializationData.GetOrCreateValue(token);
        }

        /// <summary>
        /// Determines if we have already processed the root document
        /// </summary>
        public bool HasProcessedDocumentRoot { get; set; }

        /// <summary>
        /// List of all the included items keyd by their reference
        /// </summary>
        public OrderedDictionary<ResourceObjectReference, object> Included { get; } = new OrderedDictionary<ResourceObjectReference, object>();

        /// <summary>
        /// List to keep track of which includes we have already outputted the full object serialization
        /// </summary>
        public HashSet<ResourceObjectReference> RenderedIncluded = new HashSet<ResourceObjectReference>();



    }
}
