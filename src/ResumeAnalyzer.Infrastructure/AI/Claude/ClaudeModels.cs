namespace ResumeAnalyzer.Infrastructure.AI.Claude;

public class ClaudeRequest
{
    public string Model { get; init; } = string.Empty;
    public int MaxTokens { get; init; }
    public string? System { get; init; }
    public List<ClaudeMessage> Messages { get; init; } = [];
}

public class ClaudeMessage
{
    public string Role { get; init; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class ClaudeResponse
{
    public List<ClaudeContent> Content { get; init; } = [];
}

public class ClaudeContent
{
    public string Type { get; init; } = string.Empty;
    public string Text { get; init; } = string.Empty;
}