namespace Marketplace_API_Gateway.DTOs
{
    public record class AddProductToShoppingCartDTO
    {
        public required int UserId { get; set; }
        public required int ItemId { get; set; }

        public required ProductSummaryDTO Product { get; set; }
    }
}
