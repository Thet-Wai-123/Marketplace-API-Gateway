using System.Text;
using Marketplace_API_Gateway.Config;
using Marketplace_API_Gateway.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
StartUpConfiguration.StartUpConfigure(builder);

var app = builder.Build();

//Middlewares
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
