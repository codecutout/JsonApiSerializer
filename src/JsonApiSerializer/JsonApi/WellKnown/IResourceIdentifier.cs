namespace JsonApiSerializer.JsonApi.WellKnown
{
    interface IResourceIdentifier<TResourceObject>
    {
        TResourceObject Value { get; set; }
    }
}
