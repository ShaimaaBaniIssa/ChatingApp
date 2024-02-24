using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Repository;
using API.Repository.IRepository;
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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<ColudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            return services;
        }
    }
}