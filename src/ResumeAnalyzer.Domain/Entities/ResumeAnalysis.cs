using ResumeAnalyzer.Domain.ValueObjects;

namespace ResumeAnalyzer.Domain.Entities;

public class ResumeAnalysis
{
    public Guid Id { get; private set; }
    public Guid ResumeId { get; private set; }
    public PersonelInfo PersonelInfo { get; private set; } = new();
    public IReadOnlyList<WorkExperience> WorkExperiences => _workExperiences.AsReadOnly();
    public IReadOnlyList<Education> Educations => _educations.AsReadOnly();
    public IReadOnlyList<Skill> Skills => _skills.AsReadOnly();
    public IReadOnlyList<string> Strength => _strengths.AsReadOnly();
    public IReadOnlyList<string> Weaknesses => _weaknesses.AsReadOnly();
    public IReadOnlyList<string> Suggestions => _suggestions.AsReadOnly();
    public IReadOnlyList<JobMatch> JobMatches => _jobMatches.AsReadOnly();
    public DateTime CreatedAt { get; private set; }

    private readonly List<WorkExperience> _workExperiences = [];
    private readonly List<Education> _educations = [];
    private readonly List<Skill> _skills = [];
    private readonly List<string> _strengths = [];
    private readonly List<string> _weaknesses = [];
    private readonly List<string> _suggestions = [];
    private  readonly List<JobMatch> _jobMatches = [];

    public static ResumeAnalysis Create(
        Guid resumeId,
        PersonelInfo personelInfo,
        IReadOnlyList<WorkExperience> workExperiences,
        IReadOnlyList<Education> educations,
        IReadOnlyList<Skill> skills,
        IReadOnlyList<string> strengths,
        IReadOnlyList<string> weaknesses,
        IReadOnlyList<string>  suggestions)
    {
        var analysis = new ResumeAnalysis
        {
            Id = Guid.NewGuid(),
            ResumeId = resumeId,
            PersonelInfo = personelInfo,
            CreatedAt = DateTime.UtcNow
        };
        
        analysis._workExperiences.AddRange(workExperiences);
        analysis._educations.AddRange(educations);
        analysis._skills.AddRange(skills);
        analysis._strengths.AddRange(strengths);
        analysis._weaknesses.AddRange(weaknesses);
        analysis._suggestions.AddRange(suggestions);

        return analysis;
    }

    public void AddJobMatch(JobMatch jobMatch)
    {
        _jobMatches.Add(jobMatch);
    }
}