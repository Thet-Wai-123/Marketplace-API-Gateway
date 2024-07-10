namespace Marketplace_API_Gateway.DTOs
{
    public record class UpdateProductDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public string? Description { get; set; }

        public required decimal Price { get; set; }

        public int? PostedBy { get; set; }
    }
}
