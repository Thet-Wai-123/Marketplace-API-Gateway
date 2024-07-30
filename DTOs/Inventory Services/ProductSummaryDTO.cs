using System.ComponentModel.DataAnnotations;

namespace Marketplace_API_Gateway.DTOs
{
    public record class ProductSummaryDTO
    {
        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public required decimal Price { get; set; }

        public string? PostedByName { get; set; }

        public string? PostedByID { get; set; }
    }
}
