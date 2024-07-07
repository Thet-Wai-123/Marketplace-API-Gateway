namespace Marketplace_API_Gateway.DTOs
{
    public record class ShoppingCartSummaryDTO
    {
        public required ProductSummaryDTO[] Products { get; set; }
        public decimal Total
        {
            get { return Products.Sum(p => p.Price); }
        }
    }
}
