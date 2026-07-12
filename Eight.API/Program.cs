using Eight.Api.Serialization;
using Eight.Application.DTOs.Venue;
using Eight.Application.Interfaces;
using Eight.Application.Validators;
using Eight.Domain.Entities;
using Eight.Infrastructure.Persistence;
using Eight.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, config) =>
            config.ReadFrom.Configuration(ctx.Configuration)
                  .WriteTo.Console()
                  .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));



        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

        // CORS
        builder.Services.AddCors(options =>
            options.AddPolicy("AllowAll", p =>
                p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            });
            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
        });




        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IVenueService, VenueService>();
        builder.Services.AddScoped<ITableService, TableService>();
        builder.Services.AddScoped<ISessionService, SessionService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddValidatorsFromAssemblyContaining<UserRequestValidator>();   
        builder.Services.AddValidatorsFromAssemblyContaining<VenueRequestValidator>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
                options.JsonSerializerOptions.Converters.Add(new UtcNullableDateTimeConverter());
            });



        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "text/plain";

                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                await context.Response.WriteAsync(exception?.Message ?? "Naməlum xəta");
            });
        });


        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!db.Users.Any())
            {
                db.Users.Add(new Eight.Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Email = "admin@eb.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = UserRole.SuperAdmin,
                    IsActive = true
                });
                db.SaveChanges();
            }
        }

        app.Run();
    }
}