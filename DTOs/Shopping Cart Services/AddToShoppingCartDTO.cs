using System.ComponentModel.DataAnnotations;

namespace Marketplace_API_Gateway.DTOs
{
    public record class AddToShoppingCartDTO
    {
        [Required]
        public required int ItemId { get; set; }
        public int? PostedBy { get; set; }
    }
}
