using MarketPlace_API_Gateway.Endpoints;

namespace Marketplace_API_Gateway.Endpoints
{
    public static class Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapInventoryEndpoints();
            app.MapShoppingCartEndpoints();
        }
    }
}
