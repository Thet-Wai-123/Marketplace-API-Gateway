using System.ComponentModel.DataAnnotations;

namespace MarketPlace_API_Gateway.DTOs.Inventory_Services
{
    public class GetProductDTO
    {
        [Required]
        public required int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public required decimal Price { get; set; }

        public int? PostedBy { get; set; }
    }
}
