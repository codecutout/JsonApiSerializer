namespace JsonApiSerializer.Test.Models.Products
{
    public interface IMarketedContent
    {
        string Type { get; }
        string Id { get; set; }
        string Title { get; set; }
    }
}