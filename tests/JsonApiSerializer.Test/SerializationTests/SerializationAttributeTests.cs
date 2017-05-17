using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using JsonApiSerializer.Util.JsonApiConverter.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationAttributeTests
    {

        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented, //pretty print makes it easier to debug
        };

        [Fact]
        public void When_fields_controlled_by_jsonnet_attributes_should_respect_attributes()
        {
            var root = new DocumentRoot<PersonWithAttributes>
            {
                Data = new PersonWithAttributes
                {
                    Id = "1234", //before type
                    FirstName = "John", //after lastname
                    LastName = "Smith", //before firstname
                    Twitter = "jsmi" //ignored
                }
            };

            var json = JsonConvert.SerializeObject(root,settings);

            Assert.Equal(@"
{
  ""data"": {
    ""id"": ""1234"",
    ""type"": ""people"",
    ""attributes"": {
      ""last-name"": ""Smith"",
      ""first-name"": ""John""
    }
  }
}".Trim(), json);
        }

    }
}
