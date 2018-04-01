namespace JsonApiSerializer.JsonApi.WellKnown
{
    /// <summary>
    /// Represents a Link. Links are deserialized specially to account for them being represented as either a string or an object
    /// </summary>
    public interface ILink
    {
        string Href { get; set; }
    }
}
