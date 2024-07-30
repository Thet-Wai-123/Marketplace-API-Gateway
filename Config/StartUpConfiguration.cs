using System.Text;
using MarketPlace_API_Gateway.Messaging_Queue;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace_API_Gateway.Config
{
    public static class StartUpConfiguration
    {
        public static void StartUpConfigure(WebApplicationBuilder builder)
        {
            //Secrets
            DotNetEnv.Env.Load();

            //Authentication
            builder
                .Services.AddAuthentication(cfg =>
                {
                    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTKey"))
                        ),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = Environment.GetEnvironmentVariable("JWTIssuer"),
                        ValidAudience = Environment.GetEnvironmentVariable("JWTAudience"),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddAuthorization();

            //RabbitMQ Messaging Broker
            builder.Services.AddSingleton<IQueueMethods, QueueMethods>();

            // Register HttpClient
            builder.Services.AddHttpClient();
        }
    }
}
