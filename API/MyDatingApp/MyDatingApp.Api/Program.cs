using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Helpers;
using MyDatingApp.Api.Interfaces;
using MyDatingApp.Api.Middleware;
using MyDatingApp.Api.Services;
using MyDatingApp.Api.SignalR;
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

builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddScoped<IMemberRepository, MemberRepository>();

builder.Services.AddScoped<ILikesRepository, LikesRepository>();

builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<LogUserActivity>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();
// configuring Identity..
builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>() // will cover Role based auth 
.AddEntityFrameworkStores<ApplicationDbContext>();

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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorizationBuilder().AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin")).AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator")); 

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowCredentials();// get, post, put etc..
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

app.UseStaticFiles();

app.MapControllers();

app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/messages");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    // db access request..
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await context.Connections.ExecuteDeleteAsync();
    await Seed.SeedUsers(userManager, context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();


