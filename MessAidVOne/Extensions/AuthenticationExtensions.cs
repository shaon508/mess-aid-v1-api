using System.Text;
using MassAidVOne.Domain.Utilities;
using MessAidVOne.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MessAidVOne.API.Extensions
{

    public static class AuthenticationExtensions
    {
        public static void AddAuthenticationServices(this WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddHttpContextAccessor();

            var serviceProvider = builder.Services.BuildServiceProvider();
            AppUserContext.Configure(serviceProvider.GetRequiredService<IHttpContextAccessor>());

        }
    }
}
