using ResumeAnalyzer.Domain.Enums;

namespace ResumeAnalyzer.Domain.Entities;

public class Resume
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string RawText { get; private set; } = string.Empty;
    public AnalysisStatus Status { get; private set; }
    public string? FailureReason { get; private set; }
    public string AiProvider { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ResumeAnalysis? Analysis { get; private set; }
    
    private Resume() { }

    public static Resume Create(
        string fileName,
        string contentType,
        long fileSize,
        string rawText,
        string aiProvider)
    {
        return new Resume
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            ContentType = contentType,
            FileSize = fileSize,
            RawText = rawText,
            AiProvider = aiProvider,
            Status = AnalysisStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsProcessing()
    {
        Status = AnalysisStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted(ResumeAnalysis analysis)
    {
        Status = AnalysisStatus.Completed;
        Analysis = analysis;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        Status = AnalysisStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
    
}