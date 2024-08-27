using System.ComponentModel.DataAnnotations;

namespace MarketPlace_API_Gateway.DTOs.Shopping_Cart_Services
{
    public class GetProductInfoDTO
    {
        public required int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public required decimal Price { get; set; }

        public int? PostedBy { get; set; }
    }
}
