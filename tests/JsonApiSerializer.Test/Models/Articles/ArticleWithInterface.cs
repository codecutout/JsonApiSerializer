using System.Collections.Generic;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithInterface
    {
        public interface IResourceObject
        {
            string Id { get; set; }
            string Type { get; set; }
        }

        public class CommentWithInterface : Comment, IResourceObject { }
        public class CommentReplyWithInterface : CommentReply, IResourceObject { }

        public class PersonWithInterface : Comment, IResourceObject { }
        public class PersonAdminWithInterface : PersonAdmin, IResourceObject { }

        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        public string Title { get; set; }

        public IResourceObject Author { get; set; }

        public List<IResourceObject> Comments { get; set; }

        public Links Links { get; set; }
    }
}