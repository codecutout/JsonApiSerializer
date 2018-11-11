using JsonApiSerializer.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JsonApiSerializer.Util.JsonApiConverter.Util;
using System;
using Newtonsoft.Json.Linq;

namespace JsonApiSerializer.SerializationState
{
    internal class SerializationData
    {
        private static ConditionalWeakTable<object, SerializationData> _perSerializationData = new ConditionalWeakTable<object, SerializationData>();

        public static SerializationData GetSerializationData(JsonWriter writer)
        {
            object token = writer;
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
        public bool HasProcessedDocumentRoot;

        /// <summary>
        /// List of all the included items keyed by their reference
        /// </summary>
        public readonly OrderedDictionary<ResourceObjectReference, object> Included = new OrderedDictionary<ResourceObjectReference, object>();

        /// <summary>
        /// List to keep track of which includes we have already outputted the full object serialization
        /// </summary>
        public readonly HashSet<ResourceObjectReference> RenderedIncluded = new HashSet<ResourceObjectReference>();

        /// <summary>
        /// List of actions to run after the entire document has been processed
        /// </summary>
        public readonly List<Action> PostProcessingActions = new List<Action>();

        /// <summary>
        /// Stack of converters that are processing an item. Used mostly as a hack to
        /// pass information to converters down stream
        /// </summary>
        public readonly Stack<JsonConverter> ConverterStack = new Stack<JsonConverter>();

        /// <summary>
        /// Serialization hack to keep track of what type name we used when generating references
        /// so the same typename can be used when serializing the full object
        /// </summary>
        public readonly Dictionary<object, string> ReferenceTypeNames = new Dictionary<object, string>();
    }
}
