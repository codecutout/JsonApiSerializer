namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents a Relationship.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class Relationship<TData>
    {
        public TData Data {get; set;}

        public Links Links { get; set; }

        public Meta Meta { get; set; }
    }
}
