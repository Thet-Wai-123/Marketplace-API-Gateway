using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace_API_Gateway.Config
{
    public static class StartUpConfiguration
    {
        public static void StartUpConfigure(WebApplicationBuilder builder)
        {
            DotNetEnv.Env.Load();

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
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddAuthorization();
        }
    }
}
