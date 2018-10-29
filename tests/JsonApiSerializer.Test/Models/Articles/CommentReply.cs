using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class CommentReply : Comment
    {
        [JsonProperty("response-to")]
        public Comment ResponeTo { get; set; }
    }
}