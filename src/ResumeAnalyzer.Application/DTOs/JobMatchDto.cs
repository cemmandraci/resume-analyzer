namespace ResumeAnalyzer.Application.DTOs;

public class JobMatchDto
{
    public Guid Id { get; init; }
    public string JobTitle { get; init; } = string.Empty;
    public int MatchScore { get; init; }
    public IReadOnlyList<string> MatchingSkills { get; init; } = [];
    public IReadOnlyList<string> MissingSkills { get; init; } = [];
    public string? AiFeedBack { get; init; }
    public DateTime CreatedAt { get; init; }
}