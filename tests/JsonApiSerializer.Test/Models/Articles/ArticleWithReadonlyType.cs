namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithReadonlyType : Article
    {
        public new string Type { get; } = "readonly-article-type";
    }
}
