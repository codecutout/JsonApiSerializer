using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithSerializationConditions
    {
        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        public string Title { get; set; }

        public PersonWithSerializationConditions Author { get; set; }

        public List<Comment> Comments { get; set; }

        public Links Links { get; set; }

        [JsonIgnore]
        public List<string> SerializeProperties { get; set; }

        public bool ShouldSerializeType()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Type));
        }

        public bool ShouldSerializeId()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Id));
        }

        public bool ShouldSerializeTitle()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Title));
        }

        public bool ShouldSerializeAuthor()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Author));
        }

        public bool ShouldSerializeComments()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Comments));
        }

        public bool ShouldSerializeLinks()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Links));
        }
    }
}
