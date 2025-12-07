namespace Urban.AI.Infrastructure.AI.Gemini;

#region Usings
using System.Text.Json.Serialization;
#endregion

internal sealed class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<Candidate> Candidates { get; set; } = new();
}

internal sealed class Candidate
{
    [JsonPropertyName("content")]
    public ResponseContent Content { get; set; } = new();
}

internal sealed class ResponseContent
{
    [JsonPropertyName("parts")]
    public List<ResponsePart> Parts { get; set; } = new();
}

internal sealed class ResponsePart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

internal sealed class GeminiAnalysisResult
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("subcategory")]
    public string Subcategory { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
