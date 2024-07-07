using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace_API_Gateway.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Marketplace_API_Gateway.Endpoints
{
    public static class Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("products").WithParameterValidation().RequireAuthorization();

            group.MapGet(
                "/",
                (HttpContext httpContext) =>
                {
                    var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var Name = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                    var Email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                    var Address = httpContext.User.FindFirst(ClaimTypes.StreetAddress)?.Value;
                    return Results.Ok($"User ID from token: {userId}, {Name}, {Email}, {Address}");
                }
            );
            group.MapGet("/{id}", () => { });
            group.MapPost(
                "/Post/New",
                [Authorize]
                async (CreateProductDTO newProduct) =>
                {
                    newProduct.PostedBy = 1;
                }
            );
            group.MapPut("/{id}", async (UpdateProductDTO updatedProduct) => { });
            group.MapDelete("/{id}", async (int id) => { });

            group.MapPost(
                "/ShoppingCart/Add/{id}",
                async (AddProductToShoppingCartDTO product) => { }
            );
            group.MapDelete("/ShoppingCart/Remove/{id}", async (int id) => { });
            group.MapGet(
                "/ShoppingCart",
                async () => {
                    //return ShoppingCartSummaryDTO
                }
            );
            group.MapPost("/ShoppingCart/Checkout", async () => { });
        }
    }
}
