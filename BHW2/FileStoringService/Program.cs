using FileStoringService.Data;
using Microsoft.EntityFrameworkCore;
using FileStoringService.Services;
using System;
using Microsoft.Extensions.Logging; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using FileStoringService.Services;

namespace FileStoringService
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(); 

            builder.Services.AddDbContext<FileStoringService.Data.FileAppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IFileStorageService, LocalFileSystemStorageService>();

            builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); 
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapHealthChecks("/health");
            app.MapControllers();

            app.Run();
        }
    }
}