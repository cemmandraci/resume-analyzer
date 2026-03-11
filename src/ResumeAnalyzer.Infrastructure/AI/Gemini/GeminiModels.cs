namespace ResumeAnalyzer.Infrastructure.AI.Gemini;

public class GeminiRequest
{
    public List<GeminiContent> Contents { get; init; } = [];
    public GeminiSystemInstruction? SystemInstruction { get; init; }
    public GeminiGenerationConfig? GenerationConfig { get; init; }
}


public class GeminiSystemInstruction
{
    public GeminiPart Parts { get; set; } = new();
}

public class GeminiContent
{
    public string Role { get; init; } = string.Empty;
    public List<GeminiPart> Parts { get; init; } = [];
}

public class GeminiPart
{
    public string Text { get; init; } = string.Empty;
}

public class GeminiGenerationConfig
{
    public string ResponseMimeType { get; init; } = "application/json";
}

public class GeminiResponse
{
    public List<GeminiCandidate> Candidates { get; set; } = [];
}

public class GeminiCandidate
{
    public GeminiContent Content { get; set; } = new();
}



