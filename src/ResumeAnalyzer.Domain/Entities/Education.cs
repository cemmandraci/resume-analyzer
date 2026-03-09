namespace ResumeAnalyzer.Domain.Entities;

public class Education
{
    public Guid Id { get; private set; }
    public Guid ResumeAnalysisId { get; private set; }
    public string Institution { get; private set; } = string.Empty;
    public string Degree { get; private set; } = string.Empty;
    public string Field { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public double? GPA { get; private set; }
    
    private Education() { }

    public static Education Create(
        Guid resumeAnalysisId,
        string institution,
        string degree,
        string field,
        DateTime startDate,
        DateTime? endDate = null,
        double? gpa = null)
    {
        return new Education
        {
            Id = Guid.NewGuid(),
            ResumeAnalysisId = resumeAnalysisId,
            Institution = institution,
            Degree = degree,
            Field = field,
            StartDate = startDate,
            EndDate = endDate,
            GPA = gpa
        };
    }
}