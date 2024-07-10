namespace Marketplace_API_Gateway.DTOs
{
    public record class AddProductToShoppingCartDTO
    {
        public required int ItemId { get; set; }

        public required string Name { get; set; }

        public required decimal Price { get; set; }

        public string? PostedBy { get; set; }
    }
}
