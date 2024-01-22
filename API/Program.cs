using System.Text;
using API.Data;
using API.Extensions;
using API.Services;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApllicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();
// the request is return back to client 


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("https://localhost:4200"));

app.UseAuthentication(); // check the validity of the token
app.UseAuthorization();

app.MapControllers();

app.Run();
