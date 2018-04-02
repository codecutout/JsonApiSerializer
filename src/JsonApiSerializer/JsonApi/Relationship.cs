namespace JsonApiSerializer.JsonApi
{
    public static class Relationship
    {
        /// <summary>
        /// Creates a Relationship for a given type.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        public static Relationship<TData> Create<TData>(TData data)
        {
            return new Relationship<TData>
            {
                Data = data
            };
        }
    }

    /// <summary>
    /// Represents a Relationship.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class Relationship<TData>
    {
        public TData Data { get; set; }

        public Links Links { get; set; }

        public Meta Meta { get; set; }
    }
}
