namespace ResumeAnalyzer.Domain.Interfaces;

public interface IAIServiceFactory
{
    IResumeAnalysisService Create(string provider);
    IReadOnlyList<string> GetAvailableProviders();
}