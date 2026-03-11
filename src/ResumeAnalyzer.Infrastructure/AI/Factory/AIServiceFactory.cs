using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Infrastructure.AI.Claude;
using ResumeAnalyzer.Infrastructure.AI.Gemini;

namespace ResumeAnalyzer.Infrastructure.AI.Factory;

public class AIServiceFactory : IAIServiceFactory
{
    private readonly GeminiResumeAnalysisService _geminiResumeAnalysisService;
    private readonly ClaudeResumeAnalysisService _claudeResumeAnalysisService;

    public AIServiceFactory(GeminiResumeAnalysisService geminiResumeAnalysisService, ClaudeResumeAnalysisService claudeResumeAnalysisService)
    {
        _geminiResumeAnalysisService = geminiResumeAnalysisService;
        _claudeResumeAnalysisService = claudeResumeAnalysisService;
    }

    public IResumeAnalysisService Create(string provider)
    {
        return provider.ToLower() switch
        {
            "gemini" => _geminiResumeAnalysisService,
            "claude" => _claudeResumeAnalysisService,
            _ => throw new NotSupportedException(
                $"AI provider '{provider}' is not supported. Available: Gemini, Claude")
        };
    }

    public IReadOnlyList<string> GetAvailableProviders() => ["Gemini", "Claude"];
}