using System.Text.Json.Serialization;

namespace ResumeAnalyzer.Infrastructure.AI.Gemini;

public class GeminiRequest
{
    [JsonPropertyName("contents")]
    public List<GeminiContent> Contents { get; init; } = [];
    
    [JsonPropertyName("system_instruction")]
    public GeminiSystemInstruction? SystemInstruction { get; init; }
    
    [JsonPropertyName("generation_config")]
    public GeminiGenerationConfig? GenerationConfig { get; init; }
}

public class GeminiSystemInstruction
{
    [JsonPropertyName("parts")] public List<GeminiPart> Parts { get; init; } = [];
}

public class GeminiContent
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;
    
    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; init; } = [];
}

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;
}

public class GeminiGenerationConfig
{
    [JsonPropertyName("response_mime_type")]
    public string ResponseMimeType { get; init; } = "application/json";
}

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate> Candidates { get; init; } = [];
}

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent Content { get; init; } = new();
}