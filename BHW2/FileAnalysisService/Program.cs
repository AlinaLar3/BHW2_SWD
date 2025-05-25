using FileAnalysisService.Data; // Убедитесь, что namespace для DbContext корректен
using FileAnalysisService.Services; // Убедитесь, что namespace для сервисов корректен
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Добавляем using для ILogger

namespace FileAnalysisService
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();
            builder.Services.AddDbContext<AnalysAppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddScoped<IAnalysisService, TextAnalyzer>();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AnalysAppDbContext>();
                db.Database.Migrate();
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.Run();

        }
    }
}