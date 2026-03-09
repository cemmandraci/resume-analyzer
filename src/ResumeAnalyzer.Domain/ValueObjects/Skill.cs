using ResumeAnalyzer.Domain.Enums;

namespace ResumeAnalyzer.Domain.ValueObjects;

public record Skill
{
    public string Name { get; init; } = string.Empty;
    public SkillLevel Level { get; init; } = SkillLevel.Intermediate;
    public string? Category { get; init; }
}