using AutoFixture;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.Models.Articles.VanillaJson;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationPerformanceBenchmarkTests
    {
        public SerializationPerformanceBenchmarkTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(Skip = "Used for benchmarking only")]
        [Trait("Category", "performance")]
        public void When_compared_with_standard_json_serialization_should_be_comparable()
        {
            int listSize = 100;
            TimeSpan testTime = TimeSpan.FromSeconds(30);

            var jsonApi = GenerateDocumentRoot(listSize);
            var json = MapToVanilla(jsonApi);

            //ensure we are comparing our apples with apples. Output is the same for both jsonapi and json
            var jsonApiString = JsonConvert.SerializeObject(jsonApi, new JsonApiSerializerSettings() {Formatting = Formatting.Indented});
            var jsonString = JsonConvert.SerializeObject(json, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            Assert.Equal(jsonApiString, jsonString, JsonStringEqualityComparer.InstanceIgnoreArrayOrder);

            var jsonIterations = CountIterations(testTime / 2, json, new JsonSerializerSettings());
            var jsonApiIterations = CountIterations(testTime / 2, jsonApi, new JsonApiSerializerSettings());

            output.WriteLine($"Json performed {jsonIterations} serializations in {testTime.TotalSeconds}s");
            output.WriteLine($"JsonApi performed {jsonApiIterations} serializations in {testTime.TotalSeconds}s");
            output.WriteLine($"JsonApi speed compared with Json ratio is {1.0 * jsonApiIterations / jsonIterations}");
        }

        public static DocumentRoot<List<Article>> GenerateDocumentRoot(int listSize)
        {
            var fixture = new Fixture();
            fixture.Customize<Article>(composer => composer.Without(x => x.Type).Without(x => x.Links));
            fixture.Customize<Comment>(composer => composer.Without(x => x.Type).Without(x => x.Links));
            fixture.Customize<Person>(composer => composer.Without(x => x.Type).Without(x => x.Links));
            fixture.Customize<Person>(composer => new ElementsBuilder<Person>(fixture.CreateMany<Person>(Math.Max(1, listSize / 10)).ToList()));


            return DocumentRoot.Create(fixture.CreateMany<Article>(listSize).ToList());
        }

        public static int CountIterations(TimeSpan timeout, object objToSerialize, JsonSerializerSettings settings)
        {
            var sw = new Stopwatch();
            int iterations = 0;
            JsonSerializer serializer = JsonSerializer.Create(settings);
            using (StreamWriter writer = new StreamWriter(Stream.Null))
            {
                GC.Collect();
                sw.Start();
                while(sw.Elapsed < timeout)
                {
                    //JsonConvert.SerializeObject(objToSerialize, settings);

                    serializer.Serialize(writer, objToSerialize);
                    iterations++;
                }
                sw.Stop();
            }
            return iterations;
        }

        public static DocumentRootVanillaJson<List<ArticleVanillaJson>> MapToVanilla(DocumentRoot<List<Article>> root)
        {
            var includes = new HashSet<object>();
            var rootVanilla = new DocumentRootVanillaJson<List<ArticleVanillaJson>>
            {
                Data = root.Data.Select(a => MapToVanilla(a, includes)).ToList(),
                Links = root.Links,
                Meta = root.Meta,
            };
            rootVanilla.Included = includes.ToList();

            return rootVanilla;
        }

        public static ArticleVanillaJson MapToVanilla(Article article, HashSet<object> includes)
        {
            var comments = article.Comments.Select(c => MapToVanilla(c, includes)).ToList();
            foreach (var comment in comments)
                includes.Add(comment);

            var author = MapToVanilla(article.Author, includes);
            includes.Add(author);

            return new ArticleVanillaJson
            {
                Id = article.Id,
                Type = article.Type,
                Links = article.Links,
                Attributes = new ArticleVanillaJson.ObjectAttributes
                {
                    Title = article.Title,
                },
                Relationships = new ArticleVanillaJson.ObjectRelationships
                {
                    Author = new Relationship<Reference>
                    {
                        Data = new Reference
                        {
                            Id = author.Id,
                            Type = author.Type
                        }
                    },
                    Comments = new Relationship<List<Reference>>
                    {
                        Data = comments.Select(c => new Reference
                        {
                            Id = c.Id,
                            Type = c.Type
                        }).ToList()
                    }
                }
            };
        }

        public static ConditionalWeakTable<Comment, CommentVanillaJson> PremappedComment = new ConditionalWeakTable<Comment, CommentVanillaJson>();
        public static CommentVanillaJson MapToVanilla(Comment comment, HashSet<object> includes)
        {
            if (PremappedComment.TryGetValue(comment, out var result))
                return result;

            var author = MapToVanilla(comment.Author, includes);
            includes.Add(author);

            result = new CommentVanillaJson
            {
                Id = comment.Id,
                Type = comment.Type,
                Links = comment.Links,
                Attributes = new CommentVanillaJson.ObjectAttributes
                {
                    Body = comment.Body
                },
                Relationships = new CommentVanillaJson.ObjectRelationships
                {
                    Author = new Relationship<Reference>
                    {
                        Data = new Reference
                        {
                            Id = author.Id,
                            Type = author.Type
                        }
                    }
                }
            };

            PremappedComment.Add(comment, result);
            return result;
        }


        public static ConditionalWeakTable<Person, PersonVanillaJson> PremappedPerson = new ConditionalWeakTable<Person, PersonVanillaJson>();
        private readonly ITestOutputHelper output;

        public static PersonVanillaJson MapToVanilla(Person person, HashSet<object> includes)
        {
            if (PremappedPerson.TryGetValue(person, out var result))
                return result;
            result = new PersonVanillaJson
            {
                Id = person.Id,
                Type = person.Type,
                Links = person.Links,
                Attributes = new PersonVanillaJson.ObjectAttributes
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Twitter = person.Twitter
                },
                Meta = person.Meta,
            };
            PremappedPerson.Add(person, result);
            return result;
        }
    }
}
