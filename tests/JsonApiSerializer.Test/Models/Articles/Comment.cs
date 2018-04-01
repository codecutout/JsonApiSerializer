using JsonApiSerializer.JsonApi;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class Comment
    {
        public string Type { get; set; } = "comments";

        public string Id { get; set; }

        public string Body { get; set; }

        public Person Author { get; set; }

        public Links Links { get; set; }
    }
}
