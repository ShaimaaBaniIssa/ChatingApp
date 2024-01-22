using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services
       , IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Microsoft.AspNetCore.Authentication.JwtBearer 
     .AddJwtBearer(options =>
     {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuerSigningKey = true, // check the issuer 
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
             ValidateIssuer = false,
             ValidateAudience = false
         };
     });
            return services;
        }
    }
}