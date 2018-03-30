using JsonApiSerializer.JsonApi.WellKnown;

namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents a Link.
    /// </summary>
    /// <seealso cref="JsonApiSerializer.JsonApi.WellKnown.ILink" />
    public class Link : ILink
    {
        public string Href { get; set; }

        public Meta Meta { get; set; }
    }

}
