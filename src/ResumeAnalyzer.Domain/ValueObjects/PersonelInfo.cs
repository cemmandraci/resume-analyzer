namespace ResumeAnalyzer.Domain.ValueObjects;

public record PersonelInfo
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string? LinkedInUrl { get; init; }
    public string? GitHubUrl { get; init; }
    public string? Summary { get; init; }
}