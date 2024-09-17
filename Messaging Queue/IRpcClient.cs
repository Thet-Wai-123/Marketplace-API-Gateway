using MarketPlace_API_Gateway.DTOs.Inventory_Services;

namespace MarketPlace_API_Gateway.Messaging_Queue
{
    public interface IRpcClient
    {
        public GetProductDTO ReceiveRPCResponse(string correlationId);
    }
}
