using System.IO;
using System.Reflection;

namespace JsonApiSerializer.Test.TestUtils
{
    public static class EmbeddedResource
    {
        public static string Read(string file)
        {
            var assembly = typeof(EmbeddedResource).GetTypeInfo().Assembly;
            var resourceName = string.Join(".", nameof(JsonApiSerializer), nameof(Test), file);

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
