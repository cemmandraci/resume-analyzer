namespace ResumeAnalyzer.Domain.Interfaces;

public interface IPdfParserService
{
    Task<string> ParseAsync(Stream pdfStream, CancellationToken cancellationToken = default);
}