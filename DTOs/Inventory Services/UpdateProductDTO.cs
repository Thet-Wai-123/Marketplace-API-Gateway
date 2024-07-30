using System.ComponentModel.DataAnnotations;

namespace Marketplace_API_Gateway.DTOs
{
    public record class UpdateProductDTO
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public required decimal Price { get; set; }

        public int? PostedBy { get; set; }
    }
}
