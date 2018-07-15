using JsonApiSerializer.Test.Models.Articles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class CommentReply : Comment
    {
        [JsonProperty("response-to")]
        public Comment ResponeTo { get; set; }
    }
}