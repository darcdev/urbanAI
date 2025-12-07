namespace Urban.AI.Infrastructure.AI.Gemini;

#region Usings
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Incidents;
#endregion

internal sealed class GeminiIncidentAnalysisService : IIncidentAnalysisService
{
    #region Constants
    private const string PromptTemplate = """
Eres un asistente de IA especializado en analizar imágenes de incidentes urbanos y problemas de infraestructura.

Analiza la imagen proporcionada y clasifícala según las siguientes categorías y subcategorías:

**Categorías y Subcategorías:**

A. Infraestructura Vial y Peatonal:
- Deterioro de la malla vial (huecos, grietas profundas)
- Puentes peatonales o vehiculares con fallas estructurales
- Andenes/Veredas rotas o inexistentes (obstáculos para personas con discapacidad)
- Falta de tapas de alcantarillado (alto riesgo de accidente)

B. Servicios Públicos e Iluminación:
- Luminarias fundidas o puntos ciegos de oscuridad (focos de inseguridad)
- Fugas de agua potable o desbordamiento de aguas negras
- Cables caídos o postes de energía inclinados/peligrosos
- Acumulación de basuras o ineficiencia en la recolección

C. Espacio Público y Mobiliario:
- Parques infantiles o gimnasios biosaludables en mal estado
- Bancas rotas o señalética vandalizada
- Grafitis en zonas patrimoniales o edificios públicos
- Ocupación indebida del espacio público (vendedores sin permiso que bloquean paso, terrazas ilegales)

D. Medio Ambiente y Riesgo:
- Árboles en riesgo de caída o que interfieren con redes eléctricas
- Puntos de contaminación auditiva excesiva (zonas rosas fuera de control)
- Vertimientos ilegales en cuerpos de agua

E. Movilidad y Tránsito:
- Semáforos averiados
- Señales de tránsito ocultas por vegetación o caídas
- Vehículos mal estacionados bloqueando rampas de acceso

**Instrucciones:**
1. Identifica la categoría principal (A, B, C, D o E) que mejor coincida con la imagen
2. Selecciona la subcategoría más apropiada de esa categoría
3. Proporciona una breve descripción (máximo 80 palabras) en español de lo que se visualiza en la imagen

**IMPORTANTE:**
- Si la imagen no coincide con ninguna categoría o no es un incidente urbano, responde con categoría "No aplica" y subcategoría "No aplica"
- La descripción debe ser factual y objetiva
- Enfócate en el problema de infraestructura urbana visible en la imagen

Responde SOLO con un objeto JSON válido en este formato exacto:
{
  "category": "<letra y nombre de categoría>",
  "subcategory": "<nombre exacto de subcategoría>",
  "description": "<descripción en español, máx 80 palabras>"
}
""";
    #endregion

    #region Private Members
    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;
    private readonly ILogger<GeminiIncidentAnalysisService> _logger;
    private readonly ICategoryRepository _categoryRepository;
    #endregion

    public GeminiIncidentAnalysisService(
        HttpClient httpClient,
        IOptions<GeminiOptions> options,
        ILogger<GeminiIncidentAnalysisService> logger,
        ICategoryRepository categoryRepository)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IncidentAnalysis>> AnalyzeImageAsync(
        Stream imageStream,
        CancellationToken cancellationToken)
    {
        try
        {
            var base64Image = await ConvertImageToBase64Async(imageStream, cancellationToken);

            var geminiRequest = BuildGeminiRequest(base64Image);

            var response = await SendRequestToGeminiAsync(geminiRequest, cancellationToken);

            if (response.IsFailure)
            {
                return Result.Failure<IncidentAnalysis>(response.Error);
            }

            var analysisResult = ParseGeminiResponse(response.Value);

            if (analysisResult.IsFailure)
            {
                return Result.Failure<IncidentAnalysis>(analysisResult.Error);
            }

            var incidentAnalysis = await MapToIncidentAnalysisAsync(
                analysisResult.Value,
                cancellationToken);

            return incidentAnalysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Gemini AI analysis");
            return Result.Failure<IncidentAnalysis>(IncidentErrors.AnalysisFailed);
        }
    }

    private async Task<string> ConvertImageToBase64Async(Stream imageStream, CancellationToken cancellationToken)
    {
        imageStream.Position = 0;
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private GeminiRequest BuildGeminiRequest(string base64Image)
    {
        return new GeminiRequest
        {
            Contents = new List<Content>
            {
                new()
                {
                    Parts = new List<Part>
                    {
                        new() { Text = PromptTemplate },
                        new()
                        {
                            InlineData = new InlineData
                            {
                                MimeType = "image/jpeg",
                                Data = base64Image
                            }
                        }
                    }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                Temperature = 0.4,
                TopK = 32,
                TopP = 1,
                MaxOutputTokens = 500,
                ResponseMimeType = "application/json"
            }
        };
    }

    private async Task<Result<GeminiResponse>> SendRequestToGeminiAsync(
        GeminiRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_options.Model}:generateContent";

            var jsonContent = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = httpContent
            };
            httpRequest.Headers.Add("x-goog-api-key", _options.ApiKey);

            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "Gemini API request failed with status {StatusCode}: {Error}",
                    httpResponse.StatusCode,
                    errorContent);
                return Result.Failure<GeminiResponse>(IncidentErrors.AnalysisFailed);
            }

            var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);

            if (geminiResponse is null || !geminiResponse.Candidates.Any())
            {
                _logger.LogError("Gemini API returned empty or invalid response");
                return Result.Failure<GeminiResponse>(IncidentErrors.AnalysisFailed);
            }

            return Result.Success(geminiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending request to Gemini API");
            return Result.Failure<GeminiResponse>(IncidentErrors.AnalysisFailed);
        }
    }

    private Result<GeminiAnalysisResult> ParseGeminiResponse(GeminiResponse response)
    {
        try
        {
            var firstCandidate = response.Candidates.FirstOrDefault();
            if (firstCandidate is null || !firstCandidate.Content.Parts.Any())
            {
                return Result.Failure<GeminiAnalysisResult>(IncidentErrors.AnalysisFailed);
            }

            var jsonText = firstCandidate.Content.Parts[0].Text;
            var analysisResult = JsonSerializer.Deserialize<GeminiAnalysisResult>(jsonText);

            if (analysisResult is null)
            {
                return Result.Failure<GeminiAnalysisResult>(IncidentErrors.AnalysisFailed);
            }

            return Result.Success(analysisResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Gemini response");
            return Result.Failure<GeminiAnalysisResult>(IncidentErrors.AnalysisFailed);
        }
    }

    private async Task<Result<IncidentAnalysis>> MapToIncidentAnalysisAsync(
        GeminiAnalysisResult geminiResult,
        CancellationToken cancellationToken)
    {
        if (geminiResult.Category.Equals("Not Applicable", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Success(IncidentAnalysis.CreateNotApplicable(geminiResult.Description));
        }

        var categories = await _categoryRepository.GetAllWithSubcategoriesAsync(cancellationToken);

        var categoryCode = ExtractCategoryCode(geminiResult.Category);
        var category = categories.FirstOrDefault(c =>
            c.Code.Equals(categoryCode, StringComparison.OrdinalIgnoreCase));

        if (category is null)
        {
            _logger.LogWarning(
                "Category not found for code '{CategoryCode}', marking as Not Applicable",
                categoryCode);
            return Result.Success(IncidentAnalysis.CreateNotApplicable(geminiResult.Description));
        }

        var subcategory = category.Subcategories.FirstOrDefault(s =>
            s.Name.Contains(geminiResult.Subcategory, StringComparison.OrdinalIgnoreCase) ||
            geminiResult.Subcategory.Contains(s.Name, StringComparison.OrdinalIgnoreCase));

        if (subcategory is null)
        {
            _logger.LogWarning(
                "Subcategory not found for '{SubcategoryName}' in category '{CategoryCode}', marking as Not Applicable",
                geminiResult.Subcategory,
                categoryCode);
            return Result.Success(IncidentAnalysis.CreateNotApplicable(geminiResult.Description));
        }

        var analysis = IncidentAnalysis.Create(
            category.Id,
            subcategory.Id,
            geminiResult.Description);

        return Result.Success(analysis);
    }

    private string ExtractCategoryCode(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return string.Empty;
        }

        var parts = category.Split('.');
        return parts.Length > 0 ? parts[0].Trim() : string.Empty;
    }
}
