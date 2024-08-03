using System.Text.Json;
using Marketplace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.DTOs.Shopping_Cart_Services;
using MarketPlace_API_Gateway.Messaging_Queue;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace_API_Gateway.Endpoints
{
    public static class ShoppingCartEndpoints
    {
        public static void MapShoppingCartEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("ShoppingCart")
                .RequireAuthorization()
                .WithParameterValidation();
            // GET all products in the shopping cart
            group.MapGet(
                "/{id}",
                async (HttpClient httpClient, int id) =>
                {
                    var shoppingCartResponse = await httpClient.GetAsync(
                        Environment.GetEnvironmentVariable("ShoppingCartServiceURL") + id
                    );

                    if (shoppingCartResponse.IsSuccessStatusCode)
                    {
                        var productIdsJson = shoppingCartResponse
                            .Content.ReadAsStringAsync()
                            .Result;
                        var productIds = JsonSerializer.Deserialize<List<int>>(productIdsJson);
                        var tasks = new List<Task<HttpResponseMessage>>();
                        foreach (var productId in productIds)
                        {
                            tasks.Add(
                                httpClient.GetAsync(
                                    Environment.GetEnvironmentVariable("InventoryServiceURL")
                                        + productId
                                )
                            );
                        }
                        var inventoryResponse = await Task.WhenAll(tasks);
                        List<ProductSummaryDTO> products = new();
                        foreach (var response in inventoryResponse)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var productJson = response.Content.ReadAsStringAsync().Result;
                                ProductSummaryDTO product =
                                    JsonSerializer.Deserialize<ProductSummaryDTO>(
                                        productJson,
                                        new JsonSerializerOptions
                                        {
                                            //to deal with psql returning only lower case
                                            PropertyNameCaseInsensitive = false
                                        }
                                    );
                                products.Add(product);
                            }
                        }
                        return Results.Ok(products);
                    }
                    else
                    {
                        return Results.StatusCode(503);
                    }
                }
            );

            //POST new item to Shopping Cart
            group.MapPost(
                "/Add",
                (AddToShoppingCartDTO newProduct, HttpContext httpContext, IQueueMethods queue) =>
                {
                    EndpointsHelperMethods.AddUserIdToPostedBy(httpContext, newProduct);
                    queue.SendTask("CREATE", JsonSerializer.Serialize(newProduct), "ShoppingCart");
                }
            );

            //DELETE item from Shopping Cart
            group.MapDelete(
                "/Remove/{id}",
                (
                    [FromBody] RemoveFromShoppingCartDTO removeProduct,
                    HttpContext httpContext,
                    IQueueMethods queue,
                    int id
                ) =>
                {
                    EndpointsHelperMethods.AddUserIdToPostedBy(httpContext, removeProduct);
                    removeProduct.ItemId = id;
                    queue.SendTask(
                        "DELETE",
                        JsonSerializer.Serialize(removeProduct),
                        "ShoppingCart"
                    );
                }
            );

            //POST Finilize Shopping Cart to checkout
            group.MapPost(
                "/Checkout",
                () => {
                    // 1) Confirm the payment has been made
                    // 2) Send inventory, shopping cart, and emailing services their tasks in the messaging queues
                }
            );
        }
    }
}
