# JsonApiSerializer

The JsonApiSerializer provides configurationless serializing and deserializing objects into the [json:api](http://jsonapi.org) format.

It is implemented as an `JsonSerializerSettings` for [Json.Net](http://www.newtonsoft.com/json) with usage being the standard [Json.Net](http://www.newtonsoft.com/json) methods passing in a `JsonApiSerializerSettings`

```csharp
//To serialize a POCO in json:api format
string json = JsonConvert.SerializeObject(articles, new JsonApiSerializerSettings());

//To desserialize to a POCO from json:api format
Article[] articles = JsonConvert.DeserializeObject<Article[]>(json, new JsonApiSerializerSettings());
```

## Example

Given an object model like This

```csharp
public class Article
{
    public string Id { get; set; }

    public string Title { get; set; }

    public Person Author { get; set; }

    public List<Comment> Comments { get; set; }
}

public class Comment
{
    public string Id { get; set; }

    public string Body { get; set; }

    public Person Author { get; set; }
}

public class Person
{
    public string Id { get; set; }

    [JsonProperty(propertyName: "first-name")] //uses standard Json.NET attributes to control serialization
    public string FirstName { get; set; }

    [JsonProperty(propertyName: "last-name")]
    public string LastName { get; set; }

    public string Twitter { get; set; }
}
```

and json:api content that look something like

```json
{
  "data": [{
    "type": "articles",
    "id": "1",
    "attributes": {
        "title": "JSON API paints my bikeshed!"
    },
    "relationships": {
      "author": {
        "data": { "type": "people", "id": "9" }
      },
      "comments": {
        "data": [
          { "type": "comments", "id": "5" },
          { "type": "comments", "id": "12" }
        ]
      }
    }
  }],
  "included": [{
    "type": "people",
    "id": "9",
    "attributes": {
      "first-name": "Dan",
      "last-name": "Gebhardt",
      "twitter": "dgeb"
    },
  }, {
    "type": "comments",
    "id": "5",
    "attributes": {
      "body": "First!"
    },
    "relationships": {
      "author": {
        "data": { "type": "people", "id": "2" }
      }
    },
  }, {
    "type": "comments",
    "id": "12",
    "attributes": {
      "body": "I like XML better"
    },
    "relationships": {
      "author": {
        "data": { "type": "people", "id": "9" }
      }
    },
  }]
}
```

We can desialize with 

```csharp
Article[] articles = JsonConvert.DeserializeObject<Article[]>(json, new JsonApiSerializerSettings());
```

We can also generate the JSON from our object model
```csharp
var author = new Person
{
    Id = "9",
    FirstName = "Dan",
    LastName = "Gebhardt",
    Twitter = "dgeb",
};

var articles = new[] {
    new Article
    {
        Id = "1",
        Title = "JSON API paints my bikeshed!",
        Author = author,
        Comments = new Relationship<List<Comment>>
        {
            Data = new List<Comment>
            {
                new Comment
                {
                    Id = "5",
                    Body = "First!",
                    Author = new Person
                    {
                        Type = "people",
                        Id = "2"
                    },
                },
                new Comment
                {
                    Id = "12",
                    Body = "I like XML better",
                    Author = author,
                }
            }
        }
    }
}

//will produce the same json:api json value
string json = JsonConvert.SerializeObject(articles, new JsonApiSerializerSettings());

```

## Extracting more properties
json:api allows for additional information to be stored at objects and relationships, We provide some helper classes that allows you to access these properties.

### DocumentRoot

`DocumentRoot<TData>` allows you to get and set json:api values at the root document level

```json
{
  "jsonapi": {
	"version":"1.0"
  },
  "links": {
    "self": "http://example.com/articles",
  },
  "meta": {
	"created": "2017-04-02T23:28:35"
  },
  "data": [{
  	  "id" : "1",
	  "type": "articles",
	  "attributes": {
	  	  "title": "document root example"
	  }
  }]
}
```

```csharp
DocumentRoot<Article[]> articlesRoot = JsonConvert.DeserializeObject<DocumentRoot<Article[]>>(json, new JsonApiSerializerSettings());
Assert.Equal("1.0" articlesRoot.JsonApi.Version);
Assert.Equal("http://example.com/articles", articlesRoot.Links["self"].Href);
Assert.Equal("2017-04-02T23:28:35", articlesRoot.Meta["self"]["created"]);
```

### Relationships

`Relationship<TData>` can be used in an object to get and set additional json:api around relationships

### Links

`Link` can be used to store link values. json:api supports links as either a string or as an object. JsonApiSerializer normalizes this behaviour so in the object model they are always an object.

A `Links` class is also provided to store a dictionary of links that is typically stored on json:api objects

### Types

If you dont specify a type property on your object model JsonApiSerializer will use the class name. If you want to modify this behaviour it is as simple as putting a `Type` property on a class

```csharp
public class Person
{
	public string Type { get; set; } = "people"; //sets type to "people"

	public string Id { get; set; }

	[JsonProperty(propertyName: "first-name")]
	public string FirstName { get; set; }

	[JsonProperty(propertyName: "last-name")]
	public string LastName { get; set; }

	public string Twitter { get; set; }
}
```

