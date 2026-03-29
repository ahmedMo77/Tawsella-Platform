using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Tawsella.Application;
using Tawsella.Application.AutoMapper;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Settings;
using Tawsella.Domain.Entities;
using Tawsella.Infrastructure.DbContext;
using Tawsella.Infrastructure.Repositories;
using Tawsella.Infrastructure.Services;

namespace Tawsella.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region DbContext Configuration
            // Add DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),

                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)
                )
            );
            #endregion

            #region JWT Settings and Identity Configuration
            // Bind JWT settings
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtOptions"));

            // Add JwtSettings as singleton for easy injection
            builder.Services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<JwtSettings>>().Value);

            // Add Identity & UserManager (example)
            builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
                options.SignIn.RequireConfirmedEmail = true;

                // تحويل التوكن ليكون ارقام (6-digits)
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration
                    .GetSection("JwtOptions")
                    .Get<JwtSettings>();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),

                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion

            #region CORS Policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("OpenCors", policy =>
                {
                    policy
                        .AllowAnyOrigin()     // يسمح بأي دومين
                        .AllowAnyHeader()     // يسمح بأي هيدر
                        .AllowAnyMethod();    // يسمح بأي نوع HTTP Method
                });
            });
            #endregion


            // Bind Email settings
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
                options.TokenLifespan = TimeSpan.FromMinutes(15)); // صالح لـ 15 دقيقة فقط



            // Feature-Specific Repositories
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICourierRepository, CourierRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

            // HTTP Context Accessor for Claims
            builder.Services.AddHttpContextAccessor();

            // Dependency Injection for Application Services
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IPricingService, PricingService>();
            // AutoMapper Configuration
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tawsella API", Version = "v1" });

                // JWT Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token like: Bearer {your token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("OpenCors");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
