using System;
using System.Collections.Generic;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.Models.Timer;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public partial class DeserializationCustomConverterTests
    {
        [Fact]
        public void When_custom_convertor_with_subclass_types_should_deserialize_as_types()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-with-inherited-types.json");
            var settings = new JsonApiSerializerSettings();
            settings.Converters.Add(new SubclassResourceObjectConverter<Comment>(new Dictionary<string,Type>()
            {
                { "comments-reply", typeof(CommentReply) }
            }));
            settings.Converters.Add(new SubclassResourceObjectConverter<Person>(new Dictionary<string, Type>()
            {
                { "people-admin", typeof(PersonAdmin) }
            }));


            var articles = JsonConvert.DeserializeObject<Article[]>(json, settings);

            var article = articles[0];

            Assert.IsType<Comment>(article.Comments[0]);
            Assert.IsType<Comment>(article.Comments[1]);
            Assert.IsType<CommentReply>(article.Comments[2]);
            Assert.Equal(article.Comments[1], ((CommentReply)article.Comments[2]).ResponeTo);

            Assert.IsType<PersonAdmin>(article.Author);
            Assert.Equal(new[] {"edit", "delete" }, ((PersonAdmin)article.Author).AdministratorRights);
        }

        [Fact]
        public void When_custom_convertor_with_interface_types_should_deserialize_as_types()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-with-inherited-types.json");
            var settings = new JsonApiSerializerSettings();
            settings.Converters.Add(new SubclassResourceObjectConverter<ArticleWithInterface.IResourceObject> (new Dictionary<string, Type>()
            {
                { "comments", typeof(ArticleWithInterface.CommentWithInterface) },
                { "comments-reply", typeof(ArticleWithInterface.CommentReplyWithInterface) },
                { "people", typeof(ArticleWithInterface.PersonWithInterface) },
                { "people-admin", typeof(ArticleWithInterface.PersonAdminWithInterface) }
            }));

            var articles = JsonConvert.DeserializeObject<ArticleWithInterface[]>(json, settings);

            var article = articles[0];

            Assert.IsType<ArticleWithInterface.CommentWithInterface>(article.Comments[0]);
            Assert.IsType<ArticleWithInterface.CommentWithInterface>(article.Comments[1]);
            Assert.IsType<ArticleWithInterface.CommentReplyWithInterface>(article.Comments[2]);
            Assert.Equal((ArticleWithInterface.CommentWithInterface)article.Comments[1], ((ArticleWithInterface.CommentReplyWithInterface)article.Comments[2]).ResponeTo);

            Assert.IsType<ArticleWithInterface.PersonAdminWithInterface>(article.Author);
            Assert.Equal(new[] { "edit", "delete" }, ((ArticleWithInterface.PersonAdminWithInterface)article.Author).AdministratorRights);
        }

        [Fact]
        public void When_custom_convertor_with_interface_types_not_defined_should_error()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-with-inherited-types.json");
            var settings = new JsonApiSerializerSettings();

            var error = Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<ArticleWithInterface[]>(json, settings));
        }

        [Fact]
        public void When_resource_object_custom_convertor_defined_with_attributes_should_deserialize_as_types()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-with-inherited-types.json");
            var settings = new JsonApiSerializerSettings();

            var articles = JsonConvert.DeserializeObject<ArticleWithResourceObjectCustomConvertor[]>(json, settings);

            var article = articles[0];
            Assert.IsType<PersonAdmin>(article.Author);
            Assert.Equal(new[] { "edit", "delete" }, ((PersonAdmin)article.Author).AdministratorRights);
        }

        [Fact]
        public void When_custom_convertor_should_use_on_attributes()
        {
            var json = @"{
  ""data"": {
    ""type"": ""timer"",
    ""id"": ""root"",
    ""attributes"": {
                ""duration"": 42
    },
    ""relationships"": {
                ""subTimer"": {
                    ""data"": [
                      {
            ""id"": ""sub1"",
                        ""type"": ""timer""
          }
        ]
      }
    }
  },
  ""included"": [
    {
      ""type"": ""timer"",
      ""id"": ""sub1"",
      ""attributes"": {
        ""duration"": 142
      }
    }
  ]
}";

            var settings = new JsonApiSerializerSettings();
            var timer = JsonConvert.DeserializeObject<Timer>(json, settings);

            Assert.Equal(TimeSpan.FromSeconds(42), timer.Duration);
            Assert.Single(timer.SubTimer);
            Assert.Equal(TimeSpan.FromSeconds(142), timer.SubTimer[0].Duration);
        }
    }
}