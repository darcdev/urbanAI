namespace Urban.AI.Infrastructure.AI.Gemini;

#region Usings
using System.Text.Json.Serialization;
#endregion

internal sealed class GeminiRequest
{
    [JsonPropertyName("contents")]
    public List<Content> Contents { get; set; } = new();

    [JsonPropertyName("generationConfig")]
    public GenerationConfig GenerationConfig { get; set; } = new();
}

internal sealed class Content
{
    [JsonPropertyName("parts")]
    public List<Part> Parts { get; set; } = new();
}

internal sealed class Part
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("inlineData")]
    public InlineData? InlineData { get; set; }
}

internal sealed class InlineData
{
    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}

internal sealed class GenerationConfig
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 0.4;

    [JsonPropertyName("topK")]
    public int TopK { get; set; } = 32;

    [JsonPropertyName("topP")]
    public double TopP { get; set; } = 1;

    [JsonPropertyName("maxOutputTokens")]
    public int MaxOutputTokens { get; set; } = 500;

    [JsonPropertyName("responseMimeType")]
    public string ResponseMimeType { get; set; } = "application/json";
}
