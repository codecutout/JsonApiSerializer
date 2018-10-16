using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsonApiSerializer.Test.Util
{
    public class DocumentWithLists
    {
        public string Id { get; set; }

        public Person[] Array { get; set; }

        public List<Person> List { get; set; }
    }

    public class SourcePointerTests
    {
        [Fact]
        public void When_property_expression_then_source_pointer_with_relationship_and_attributes()
        {
            var pointer = SourcePointer.FromModel<Article>(x => x.Author.FirstName, new JsonApiSerializerSettings());
            Assert.Equal("/data/relationships/author/data/attributes/first-name", pointer);
        }

        [Fact]
        public void When_document_property_expression_then_source_pointer_with_relationship_and_attributes()
        {
            var pointer = SourcePointer.FromModel<IDocumentRoot<Article>>(x => x.Data.Author.FirstName, new JsonApiSerializerSettings());
            Assert.Equal("/data/relationships/author/data/attributes/first-name", pointer);
        }

        [Fact]
        public void When_expression_requires_evaluation_then_source_pointer_evaluated()
        {
            var one = 1;
            var pointer = SourcePointer.FromModel<DocumentWithLists>(x => x.Array[21 + one].FirstName, new JsonApiSerializerSettings());
            Assert.Equal("/data/relationships/array/data/22/attributes/first-name", pointer);
        }

        [Fact]
        public void When_expression_array_index_then_source_pointer_with_list_access()
        {
            var pointer = SourcePointer.FromModel<DocumentWithLists>(x => x.Array[22].FirstName, new JsonApiSerializerSettings());
            Assert.Equal("/data/relationships/array/data/22/attributes/first-name", pointer);
        }

        [Fact]
        public void When_expression_list_index_then_source_pointer_with_list_access()
        {
            var pointer = SourcePointer.FromModel<DocumentWithLists>(x => x.List[22].FirstName, new JsonApiSerializerSettings());
            Assert.Equal("/data/relationships/list/data/22/attributes/first-name", pointer);
        }

        [Fact]
        public void When_expression_dictionary_index_then_source_pointer_with_property_access()
        {
            var pointer = SourcePointer.FromModel<DocumentRoot<Article>>(x => x.Meta["lastUpdated"], new JsonApiSerializerSettings());
            Assert.Equal("/meta/lastUpdated", pointer);
        }

        [Theory]
        [InlineData(typeof(Article), "x.Title", "/data/attributes/title")]
        [InlineData(typeof(Article), "x.Author.FirstName", "/data/relationships/author/data/attributes/first-name")]
        [InlineData(typeof(IDocumentRoot<Article>), "x.Data.Author.FirstName", "/data/relationships/author/data/attributes/first-name")]

        [InlineData(typeof(DocumentRoot<Article>), "x.JsonApi.Version", "/jsonapi/version")]
        [InlineData(typeof(DocumentRoot<Article>), "x.Meta['lastUpdated']", "/meta/lastUpdated")]
        [InlineData(typeof(DocumentRoot<Article>), "x.Meta[\"lastUpdated\"]", "/meta/lastUpdated")]

        [InlineData(typeof(Article), "x.Comments[22].Body", "/data/relationships/comments/data/22/attributes/body")]

        [InlineData(typeof(ArticleWithRelationship), "x.Author.Data.FirstName", "/data/relationships/author/data/attributes/first-name")]
        [InlineData(typeof(ArticleWithRelationship), "x.Author.Meta", "/data/relationships/author/meta")]
        public void When_model_path_should_map_to_json_path(Type modelType, string modelPath, string expectedJsonPath)
        {
            var jsonPath = SourcePointer.FromModel(modelType, modelPath, new JsonApiSerializerSettings());
            Assert.Equal(expectedJsonPath, jsonPath);
        }
    }
}
