using L_Bank_W_Backend.Core;
using L_Bank_W_Backend.DbAccess;
using L_Bank_W_Backend.DbAccess.Repositories;
using L_Bank_W_Backend.Models;
using L_Bank_W_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// https://www.prowaretech.com/articles/current/asp-net-core/add-jwt-authentication-to-mvc#!

namespace L_Bank_W_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings")
            );
            
            builder.Services.Configure<DatabaseSettings>(
                builder.Configuration.GetSection("DatabaseSettings")
            );
            
            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
                
                if(jwtSettings == null || jwtSettings.Issuer == null || jwtSettings.Audience == null || jwtSettings.IssuerSigninKey == null)
                {
                    throw new Exception("JwtSettings not found or not complete in appsettings.json");
                }
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = jwtSettings.Audience, // NOTE: USE THE REAL DOMAIN NAME
                    ValidIssuer = jwtSettings.Issuer, // NOTE: USE THE REAL DOMAIN NAME
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(jwtSettings.IssuerSigninKey!)) // NOTE: THIS SHOULD BE A SECRET KEY NOT TO BE SHARED; REPLACE THIS GUID WITH A UNIQUE ONE
                };
            });

            builder.Services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
            builder.Services.AddTransient<ILedgerRepository, LedgerRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<ILoginService, LoginService>();
            builder.Services.AddTransient<IBookingRepository, BookingRepository>();
            
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo()
                    {
                        Title = "My API - V1",
                        Version = "v1"
                    }
                );
            });
            
            builder.Services.AddAuthorization();

            var app = builder.Build();
            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
                });
            }

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();

            // For index.html
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            //app.MapHub<ChangedHub>("/changedHub");

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    // Example: Run a startup task
                    var databaseSeeder = services.GetRequiredService<IDatabaseSeeder>();     
                    Console.WriteLine("Initializing database.");
                    databaseSeeder.Initialize();
                    Console.WriteLine("Seeding data.");
                    databaseSeeder.Seed();
                }
                catch (Exception ex)
                {
                    // Log exceptions or handle errors
                    Console.WriteLine($"Error during startup: {ex.Message}");
                }
            }
            
            app.Run();
        }
    }
}