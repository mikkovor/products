namespace Products.Models.Requests
{
    public record ProductsRequest(string SortBy = "Name", bool SortDescending = false);
}