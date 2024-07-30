using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Marketplace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.DTOs.Shopping_Cart_Services;
using MarketPlace_API_Gateway.Endpoints;
using MarketPlace_API_Gateway.Messaging_Queue;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace_API_Gateway.Endpoints
{
    public static class Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("").RequireAuthorization().WithParameterValidation();

            //GET all items in Inventory
            group.MapGet(
                "/Inventory",
                async (HttpClient httpClient) =>
                {
                    //replace this with an url from appsetting.json or dotenv with CORS set up to only accept that incoming url
                    var response = await httpClient.GetAsync("https://localhost:7194/Products");
                    if (response.IsSuccessStatusCode)
                    {
                        return Results.Ok(response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        return Results.StatusCode(503);
                    }
                }
            );

            //GET Inventory by ID
            group.MapGet(
                "/Inventory/{id}",
                async (HttpClient httpClient, int id) =>
                {
                    var response = await httpClient.GetAsync(
                        Environment.GetEnvironmentVariable("InventoryServiceURL") + id
                    );
                    if (response.IsSuccessStatusCode)
                    {
                        return Results.Ok(response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        return Results.StatusCode(503);
                    }
                }
            );

            //POST new item to Inventory
            group.MapPost(
                "/Inventory/New",
                (CreateProductDTO newProduct, HttpContext httpContext, IQueueMethods queue) =>
                {
                    EndpointsHelperMethods.AddUserIdToPostedBy(httpContext, newProduct);
                    queue.SendTask("CREATE", JsonSerializer.Serialize(newProduct), "Inventory");
                }
            );

            //PUT update item in Inventory
            group.MapPut(
                "/Inventory/Update/{id}",
                (
                    [FromBody] UpdateProductDTO updatedProduct,
                    HttpContext httpContext,
                    IQueueMethods queue,
                    int id
                ) =>
                {
                    EndpointsHelperMethods.AddUserIdToPostedBy(httpContext, updatedProduct);
                    updatedProduct.Id = id;
                    queue.SendTask("UPDATE", JsonSerializer.Serialize(updatedProduct), "Inventory");
                }
            );

            //DELETE item in Inventory
            group.MapDelete(
                "/Inventory/Delete/{id}",
                (
                    [FromBody] DeleteProductDTO deleteProduct,
                    HttpContext httpContext,
                    IQueueMethods queue,
                    int id
                ) =>
                {
                    EndpointsHelperMethods.AddUserIdToPostedBy(httpContext, deleteProduct);
                    deleteProduct.Id = id;
                    queue.SendTask("DELETE", JsonSerializer.Serialize(deleteProduct), "Inventory");
                }
            );

            // GET all products in the shopping cart
            group.MapGet(
                "/ShoppingCart/{id}",
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
                "/ShoppingCart/Add",
                (AddToShoppingCartDTO newProduct, HttpContext httpContext, IQueueMethods queue) =>
                {
                    EndpointsHelperMethods.AddUserIdToPostedBy(httpContext, newProduct);
                    queue.SendTask("CREATE", JsonSerializer.Serialize(newProduct), "ShoppingCart");
                }
            );

            //DELETE item from Shopping Cart
            group.MapDelete(
                "/ShoppingCart/Remove/{id}",
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
                "/ShoppingCart/Checkout",
                () => {
                    // 1) Confirm the payment has been made
                    // 2) Send inventory, shopping cart, and emailing services their tasks in the messaging queues
                }
            );
        }
    }
}
