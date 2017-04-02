using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Test.TestUtils
{
    public static class EmbeddedResource
    {
        public static string Read(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Join(".", nameof(JsonApiSerializer), nameof(Test), file);

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
