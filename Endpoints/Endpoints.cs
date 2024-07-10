using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using Marketplace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.Messaging_Queue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace_API_Gateway.Endpoints
{
    public static class Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("").WithParameterValidation().RequireAuthorization();

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
            group.MapGet(
                "/Inventory/{id}",
                async (HttpClient httpClient, int id) =>
                {
                    //replace this with an url from appsetting.json or dotenv with CORS set up to only accept that incoming url
                    var response = await httpClient.GetAsync(
                        $"https://localhost:7194/Products/{id}"
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
            group.MapPost(
                "/Inventory/New",
                (CreateProductDTO newProduct, HttpContext httpContext, IQueueMethods queue) =>
                {
                    AddUserIdToPostedBy(httpContext, newProduct);
                    queue.SendTask("CREATE", JsonSerializer.Serialize(newProduct), "Inventory");
                }
            );
            group.MapPut(
                "/Inventory/Update/{id}",
                (
                    UpdateProductDTO updatedProduct,
                    HttpContext httpContext,
                    IQueueMethods queue,
                    int id
                ) =>
                {
                    AddUserIdToPostedBy(httpContext, updatedProduct);
                    updatedProduct.Id = id;
                    queue.SendTask("UPDATE", JsonSerializer.Serialize(updatedProduct), "Inventory");
                }
            );
            group.MapDelete(
                "/Inventory/Delete/{id}",
                (
                    [FromBody] DeleteProductDTO deleteProduct,
                    HttpContext httpContext,
                    IQueueMethods queue,
                    int id
                ) =>
                {
                    AddUserIdToPostedBy(httpContext, deleteProduct);
                    deleteProduct.Id = id;
                    queue.SendTask("DELETE", JsonSerializer.Serialize(deleteProduct), "Inventory");
                }
            );

            group.MapPost(
                "/ShoppingCart/Add",
                (
                    AddProductToShoppingCartDTO newProduct,
                    HttpContext httpContext,
                    IQueueMethods queue
                ) =>
                {
                    AddUserIdToPostedBy(httpContext, newProduct);
                    queue.SendTask("CREATE", JsonSerializer.Serialize(newProduct), "ShoppingCart");
                }
            );
            group.MapDelete("/ShoppingCart/Remove/{id}", (int id) => { });
            group.MapGet(
                "/ShoppingCart",
                async () => {
                    //return ShoppingCartSummaryDTO
                }
            );
            group.MapPost("/ShoppingCart/Checkout", () => { });
        }

        private static void AddUserIdToPostedBy(HttpContext httpContext, dynamic taskInfo)
        {
            var userId = Int32.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            taskInfo.PostedBy = userId;
        }
    }
}
