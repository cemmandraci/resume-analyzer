namespace ResumeAnalyzer.Application.DTOs;

public class ResumeAnalysisDto
{
    public Guid Id { get; set; }
    public PersonelInfoDto PersonelInfoDto { get; init; } = new();
    public IReadOnlyList<WorkExperienceDto> WorkExperiences { get; init; } = [];
    public IReadOnlyList<EducationDto> Educations { get; init; } = [];
    public IReadOnlyList<SkillDto> Skills { get; init; } = [];
    public IReadOnlyList<string> Strengths { get; init; } = [];
    public IReadOnlyList<string> Weaknesses { get; init; } = [];
    public IReadOnlyList<string> Suggestions { get; init; } = [];
    public IReadOnlyList<JobMatchDto> JobMatches { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class PersonelInfoDto
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Location  { get; init; } = string.Empty;
    public string? LinkedInUrl { get; init; }
    public string? GitHubUrl { get; init; }
    public string? Summary { get; init; }
}

public class WorkExperienceDto
{
    public string Company { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public IReadOnlyList<string> Responsibilities { get; init; } = [];
}

public class EducationDto
{
    public string Institution { get; init; } = string.Empty;
    public string Degree { get; init; } = string.Empty;
    public string Field { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public double? GPA { get; init; }
}

public class SkillDto
{
    public string Name { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public string? Category { get; init; }
}