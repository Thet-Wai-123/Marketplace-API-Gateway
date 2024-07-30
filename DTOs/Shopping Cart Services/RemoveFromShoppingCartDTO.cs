namespace MarketPlace_API_Gateway.DTOs.Shopping_Cart_Services
{
    public class RemoveFromShoppingCartDTO
    {
        public int ItemId { get; set; }
        public int? PostedBy { get; set; }
    }
}
