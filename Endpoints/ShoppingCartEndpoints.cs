using System.Text.Json;
using Marketplace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.DTOs.Shopping_Cart_Services;
using MarketPlace_API_Gateway.Messaging_Queue;
using Microsoft.AspNetCore.Components.Forms.Mapping;
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
                "/",
                //needs the httpClient for sending requests to other services, and httpContext for getting the user id
                async (HttpClient httpClient, HttpContext httpContext) =>
                {
                    var result = await GetProductsInShoppingCart(httpClient, httpContext);
                    if (result != null)
                    {
                        return Results.Ok(result);
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

            //POST Finalize Shopping Cart to checkout
            group.MapPost(
                "/Checkout",
                async (HttpClient httpClient, HttpContext httpContext, IQueueMethods queue) =>
                {
                    // 1) Confirm the payment has been made

                    // 2) Get all products in the shopping cart, and we delete them in both the shopping cart and the inventory service
                    var products = await GetProductsInShoppingCart(httpClient, httpContext);

                    foreach (var product in products)
                    {
                        DeleteProductDTO inventoryProductToDelete =
                            new() { Id = product.Id, PostedBy = product.PostedBy.Value };
                        queue.SendTask(
                            "DELETE",
                            JsonSerializer.Serialize(inventoryProductToDelete),
                            "Inventory"
                        );

                        RemoveFromShoppingCartDTO shoppingCartProductToDelete =
                            new() { ItemId = product.Id, PostedBy = product.PostedBy.Value };
                        queue.SendTask(
                            "DELETE",
                            JsonSerializer.Serialize(shoppingCartProductToDelete),
                            "ShoppingCart"
                        );
                    }
                }
            );
        }

        private static async Task<List<GetProductInfoDTO>> GetProductsInShoppingCart(
            HttpClient httpClient,
            HttpContext httpContext
        )
        {
            int myId = EndpointsHelperMethods.GetUserId(httpContext);
            var shoppingCartResponse = await httpClient.GetAsync(
                Environment.GetEnvironmentVariable("ShoppingCartServiceURL") + myId
            );

            if (shoppingCartResponse.IsSuccessStatusCode)
            {
                var productIdsJson = shoppingCartResponse.Content.ReadAsStringAsync().Result;
                var productIds = JsonSerializer.Deserialize<List<int>>(productIdsJson);
                var tasks = new List<Task<HttpResponseMessage>>();
                foreach (var productId in productIds)
                {
                    tasks.Add(
                        httpClient.GetAsync(
                            Environment.GetEnvironmentVariable("InventoryServiceURL") + productId
                        )
                    );
                }
                var inventoryResponse = await Task.WhenAll(tasks);
                List<GetProductInfoDTO> products = new();
                foreach (var response in inventoryResponse)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var productJson = response.Content.ReadAsStringAsync().Result;
                        GetProductInfoDTO product = JsonSerializer.Deserialize<GetProductInfoDTO>(
                            productJson,
                            new JsonSerializerOptions
                            {
                                //to deal with psql returning only lower case
                                PropertyNameCaseInsensitive = true
                            }
                        );
                        products.Add(product);
                    }
                }
                return products;
            }
            else
            {
                return null;
            }
        }
    }
}
