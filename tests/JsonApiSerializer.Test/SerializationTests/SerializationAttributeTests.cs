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



        [Fact]
        public void When_fields_controlled_by_jsonnet_nullinclude_attributes_should_include_null()
        {
            var root = new DocumentRoot<ArticleWithNullIncludeProperties>
            {
                Data = new ArticleWithNullIncludeProperties
                {
                    Id = "1234", 
                    Title = null, //default NullValueHandling
                    Author = null, //NullValueHandling.Ignore
                    Comments = null //NullValueHandling.Include
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"
{
  ""data"": {
    ""type"": ""articles"",
    ""id"": ""1234"",
    ""attributes"": {
      ""comments"": null
    }
  }
}".Trim(), json);
        }

        [Fact]
        public void When_fields_controlled_by_jsonnet_nullignore_attributes_should_ignore_null()
        {
            var root = new DocumentRoot<ArticleWithNullIncludeProperties>
            {
                Data = new ArticleWithNullIncludeProperties
                {
                    Id = "1234",
                    Title = null, //default NullValueHandling
                    Author = null, //NullValueHandling.Ignore
                    Comments = null //NullValueHandling.Include
                }
            };


            var newSettings = new JsonApiSerializerSettings()
            {
                Formatting = Formatting.Indented, //pretty print makes it easier to debug
                NullValueHandling = NullValueHandling.Include
            };
            var json = JsonConvert.SerializeObject(root, newSettings);

            Assert.Equal(@"
{
  ""data"": {
    ""type"": ""articles"",
    ""id"": ""1234"",
    ""links"": null,
    ""attributes"": {
      ""title"": null,
      ""comments"": null
    }
  }
}".Trim(), json);
        }

    }
}
