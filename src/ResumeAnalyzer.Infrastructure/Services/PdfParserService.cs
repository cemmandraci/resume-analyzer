using System.Text;
using ResumeAnalyzer.Domain.Interfaces;
using UglyToad.PdfPig;

namespace ResumeAnalyzer.Infrastructure.Services;

public class PdfParserService : IPdfParserService
{
    public Task<string> ParseAsync(Stream pdfStream, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        pdfStream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();

        using var document = PdfDocument.Open(bytes);
        var textBuilder = new StringBuilder();

        foreach (var page in document.GetPages())
        {
            var words = page.GetWords();
            textBuilder.AppendLine(string.Join(" ", words.Select(w => w.Text)));
        }
        
        var text = textBuilder.ToString().Trim();

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException(
                "Could not extract text from PDF. The file may be scanned or image-based.");
        
        return Task.FromResult(text);
    }
}