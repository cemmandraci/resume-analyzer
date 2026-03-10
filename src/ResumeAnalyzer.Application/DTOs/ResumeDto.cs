namespace ResumeAnalyzer.Application.DTOs;

public class ResumeDto
{
    public Guid Id { get; set; }
    public string FileName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string Status { get; init; } = string.Empty;
    public string AiProvider { get; init; } = string.Empty;
    public string? FailureReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public ResumeAnalysisDto? Analysis { get; set; }
    
    
}