using JsonApiSerializer.JsonConverters;
using JsonApiSerializer.ReferenceResolvers;
using Newtonsoft.Json;

namespace JsonApiSerializer
{
    public static class JsonSerializerSettingsExtensions
    {
        public static void AddJsonApiConverter(this JsonSerializerSettings settings, JsonConverter resourceObjectConverter)
        {
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            settings.ReferenceResolverProvider = () => new IncludedReferenceResolver();
            settings.ContractResolver = new JsonApiContractResolver();

            settings.Converters.Add(new ResourceWrapConverter(resourceObjectConverter));
            settings.Converters.Add(new ResourceListWrapConverter(resourceObjectConverter));
        }
    }
}
