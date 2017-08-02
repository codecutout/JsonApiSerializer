using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.Util.JsonApiConverter.Util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.ReferenceResolvers
{
    internal class IncludedReferenceResolver : OrderedDictionary<string, object>, IReferenceResolver
    {
        public static string RootReference = "root";

        public static string GetReferenceValue(string id, string type)
        {
            return $"{type}:{id}";
        }

        public static string GetReferenceValue(JObject jobj)
        {
            return GetReferenceValue(jobj[PropertyNames.Id].ToString(), jobj[PropertyNames.Type].ToString());
        }

        /// <summary>
        /// List to keep track of which references we have outputted during serialization
        /// </summary>
        public HashSet<string> RenderedReferences = new HashSet<string>();

        public void AddReference(object context, string reference, object value)
        {
            this[reference] = value;
        }

        public string GetReference(object context, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsReferenced(object context, object value)
        {
            throw new NotImplementedException();
        }

        public object ResolveReference(object context, string reference)
        {
            object result;
            this.TryGetValue(reference, out result);
            return result;
        }
    }
}
