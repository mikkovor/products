namespace Products.Models.Requests
{
    public record ProductsRequest
    {
        public string SortBy { get; init; } = "Name";
        public bool SortDescending { get; init; }
    }
}