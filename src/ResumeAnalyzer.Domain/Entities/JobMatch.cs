namespace ResumeAnalyzer.Domain.Entities;

public class JobMatch
{
    public Guid Id { get; private set; }
    public Guid ResumeAnalysisId { get; private set; }
    public string JobTitle { get; private set; } = string.Empty;
    public string JobDescription { get; private set; } = string.Empty;
    public int MatchScore { get; private set; }
    public IReadOnlyList<string> MatchingSkills => _matchingSkills.AsReadOnly();
    public IReadOnlyList<string> MissingSkills => _missingSkills.AsReadOnly();
    public string? AIFeedBack { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<string> _matchingSkills = [];
    private readonly List<string> _missingSkills = [];
    
    private JobMatch() { }

    public static JobMatch Create(
        Guid resumeAnalysisId,
        string jobTitle,
        string jobDescription,
        int matchScore,
        IReadOnlyList<string> matchingSkills,
        IReadOnlyList<string>  missingSkills,
        string? aiFeedBack = null)
    {
        var jobMatch = new JobMatch
        {
            Id = Guid.NewGuid(),
            ResumeAnalysisId = resumeAnalysisId,
            JobTitle = jobTitle,
            JobDescription = jobDescription,
            MatchScore = matchScore,
            AIFeedBack = aiFeedBack,
            CreatedAt = DateTime.UtcNow
        };
        
        jobMatch._matchingSkills.AddRange(matchingSkills);
        jobMatch._missingSkills.AddRange(missingSkills);
        
        return jobMatch;
    }
    
}