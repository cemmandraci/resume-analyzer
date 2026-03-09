namespace ResumeAnalyzer.Domain.Entities;

public class WorkExperience
{
    public Guid Id { get; private set; }
    public Guid ResumeAnalysisId { get; private set; }
    public string Company { get; private set; } = string.Empty;
    public string Position { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsCurrent => EndDate == null;
    public IReadOnlyList<string> Responsibilities => _responsibilities.AsReadOnly();

    private readonly List<string> _responsibilities = [];
    
    private WorkExperience() { }

    public static WorkExperience Create(
        Guid resumeAnalysisId,
        string company,
        string position,
        DateTime startDate,
        DateTime? endDate = null,
        string? description = null,
        IReadOnlyList<string>? responsibilities = null)
    {
        var experience = new WorkExperience
        {
            Id = Guid.NewGuid(),
            ResumeAnalysisId = resumeAnalysisId,
            Company = company,
            Position = position,
            StartDate = startDate,
            EndDate = endDate,
            Description = description,
        };
        
        if(responsibilities != null)
            experience._responsibilities.AddRange(responsibilities);
        
        return experience;
    }
}