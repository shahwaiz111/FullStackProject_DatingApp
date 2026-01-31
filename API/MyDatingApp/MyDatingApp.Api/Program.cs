using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.Interfaces;
using MyDatingApp.Api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatingAppCon"));
});

// Adding CORS
builder.Services.AddCors();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var tokenkey = builder.Configuration["tokenkey"] ?? throw new Exception("Token key not found..");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenkey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod(); // get, post, put etc..
    options.WithOrigins("http://localhost:4200", "https://localhost:4200");
});

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
