using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithReadonlyType : Article
    {
        public new string Type { get; } = "readonly-article-type";
    }
}
