using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Marketplace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.DTOs;
using MarketPlace_API_Gateway.Messaging_Queue;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace_API_Gateway.Endpoints
{
    public static class InventoryEndpoints
    {
        public static void MapInventoryEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("Inventory").RequireAuthorization().WithParameterValidation();
            //GET all items in Inventory
            group.MapGet(
                "/",
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
                "/{id}",
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
                "/New",
                (CreateProductDTO newProduct, HttpContext httpContext, IQueueMethods queue) =>
                {
                    EndpointsHelperMethods.AddUserIdToPostedBy(httpContext, newProduct);
                    queue.SendTask("CREATE", JsonSerializer.Serialize(newProduct), "Inventory");
                }
            );

            //PUT update item in Inventory
            group.MapPut(
                "/Update/{id}",
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
                "/Delete/{id}",
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

            //POST search for item in Inventory
            group.MapPost(
                "/query",
                (
                    [FromQuery] [Required] string keyword,
                    [FromQuery] int minPrice,
                    HttpClient httpClient
                ) =>
                {
                    if (keyword == null)
                    {
                        return Results.BadRequest("Keyword is required");
                    }
                    string[] breakWords = ["the", "a", "is", "on"];
                    StringBuilder sb = new(keyword);
                    foreach (string breakWord in breakWords)
                    {
                        sb.Replace(breakWord, "");
                    }
                    var queries = httpClient.PostAsJsonAsync(
                        Environment.GetEnvironmentVariable("InventoryServiceURL") + "query",
                        sb.ToString()
                    );
                    return Results.Ok(queries);
                    //    .split(/\s+/)
                    //    .join("|");
                }
            );
        }
    }
}
