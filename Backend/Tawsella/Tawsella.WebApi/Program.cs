using Tawsella.Application;
using Tawsella.Application.Contracts.Services;
using Tawsella.Infrastructure;
using Tawsella.Infrastructure.Services;

namespace Tawsella.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Extensions Methods (Infrastructure & Application)
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices(builder.Configuration);

            // 2. Web-Specific (API Level only)
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddCors(options => {
                options.AddPolicy("OpenCors", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger Configuration
            builder.Services.AddSwaggerGen(c => {
                // ... (Your Swagger Code)
            });

            var app = builder.Build();

            // 3. Pipeline
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
