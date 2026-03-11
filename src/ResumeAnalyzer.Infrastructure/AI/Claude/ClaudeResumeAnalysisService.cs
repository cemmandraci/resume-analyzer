using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ResumeAnalyzer.Infrastructure.AI.Base;

namespace ResumeAnalyzer.Infrastructure.AI.Claude;

public class ClaudeResumeAnalysisService : BaseResumeAnalysisService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private const string Model = "claude-sonnet-4-6";

    public ClaudeResumeAnalysisService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _ = configuration["AI:Claude:ApiKey"]
            ?? throw new InvalidOperationException("Claude API key is not configured");
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public override string ProviderName => "Claude";
    
    protected override async Task<string> SendRequestAsync(string prompt, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("Claude");

        var request = new ClaudeRequest
        {
            Model = Model,
            MaxTokens = 4096,
            System =
                "You are an expert HR analyst and career coach.Always respond with valid JSON only, no markdown, no extra text.",
            Messages = [new ClaudeMessage { Role = "user", Content = prompt }]
        };
        
        var json = JsonSerializer.Serialize((request, _jsonSerializerOptions));
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("v1/messages", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var claudeResponse = JsonSerializer.Deserialize<ClaudeResponse>(responseBody, _jsonSerializerOptions)
                             ?? throw new InvalidOperationException("Could not parse Claude response.");

        return claudeResponse.Content.FirstOrDefault(c => c.Type == "text")?.Text
               ?? throw new InvalidOperationException("No response from Claude");

    }
}