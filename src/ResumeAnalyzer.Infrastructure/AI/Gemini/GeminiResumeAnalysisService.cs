using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Enums;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Domain.ValueObjects;
using ResumeAnalyzer.Infrastructure.AI.Base;

namespace ResumeAnalyzer.Infrastructure.AI.Gemini;

public class GeminiResumeAnalysisService : BaseResumeAnalysisService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private const string Model = "gemini-2.5-flash";

    public GeminiResumeAnalysisService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration["AI:Gemini:ApiKey"]
                  ?? throw new InvalidOperationException("Gemini API key is not configured");
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
    }

    public override string ProviderName => "Gemini";
    
    protected async override Task<string> SendRequestAsync(string prompt, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("Gemini");

        var request = new GeminiRequest
        {
            SystemInstruction = new GeminiSystemInstruction
            {
                Parts = new GeminiPart
                {
                    Text =
                        "You are an expert HR analyst and career coach.Always respond with valid JSON only, no markdown, no extra text."
                }
            },
            Contents =
            [
                new GeminiContent
                {
                    Role = "user",
                    Parts = [new GeminiPart { Text = prompt }]
                }
            ],
            GenerationConfig = new GeminiGenerationConfig
            {
                ResponseMimeType = "application/json"
            }
        };

        var json = JsonSerializer.Serialize((request, _jsonSerializerOptions));
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var url = $"v1beta/models/{Model}:generateContent?key={_apiKey}";
        var response = await client.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseBody, _jsonSerializerOptions)
                             ?? throw new InvalidOperationException("Could not parse Gemini response");

        return geminiResponse.Candidates.FirstOrDefault()?.Content.Parts.FirstOrDefault()?.Text
               ?? throw new InvalidOperationException("No response from Gemini");
    }
}