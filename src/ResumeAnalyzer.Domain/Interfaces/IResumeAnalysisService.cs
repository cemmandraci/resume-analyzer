using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.ValueObjects;

namespace ResumeAnalyzer.Domain.Interfaces;

public interface IResumeAnalysisService
{
    string ProviderName { get; }
    Task<ResumeAnalysisResult> AnalyzeAsync(string resumeText, CancellationToken cancellationToken = default);
    Task<JobMatchResult> MatchWithJobAsync(string resumeText, string jobDescription, CancellationToken cancellationToken = default);
}

public record JobMatchResult(
    int MatchScore,
    IReadOnlyList<string> MatchingSkills,
    IReadOnlyList<string> MissingSkills,
    string AIFeedback);

public record ResumeAnalysisResult(
    PersonelInfo PersonelInfo,
    IReadOnlyList<WorkExperience> WorkExperiences,
    IReadOnlyList<Education> Educations,
    IReadOnlyList<Skill> Skills,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> Weaknesses,
    IReadOnlyList<string> Suggestions);
    
    