using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Services;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApllicationServices(this IServiceCollection services
        , IConfiguration config)
        {
            services.AddDbContext<DataContext>(option =>
        {
            option.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });
            services.AddCors();


            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}