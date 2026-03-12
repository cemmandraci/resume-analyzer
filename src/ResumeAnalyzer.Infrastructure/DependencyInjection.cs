using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.AI.Claude;
using ResumeAnalyzer.Infrastructure.AI.Factory;
using ResumeAnalyzer.Infrastructure.AI.Gemini;
using ResumeAnalyzer.Infrastructure.Persistence;
using ResumeAnalyzer.Infrastructure.Persistence.Repositories;
using ResumeAnalyzer.Infrastructure.Services;

namespace ResumeAnalyzer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        services.AddDbContext<ResumeDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpClient("Gemini", client =>
        {
            client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        
        services.AddHttpClient("Claude", client =>
        {
            client.BaseAddress = new Uri("https://api.anthropic.com/");
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Add("x-api-key", configuration["AI:Claude:ApiKey"]);
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        });

        services.AddScoped<GeminiResumeAnalysisService>();
        services.AddScoped<ClaudeResumeAnalysisService>();

        services.AddScoped<IAIServiceFactory, AIServiceFactory>();

        services.AddScoped<IPdfParserService, PdfParserService>();
        services.AddScoped<IResumeRepository, ResumeRepository>();

        return services;
    }
}