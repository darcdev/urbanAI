namespace Urban.AI.WebApi.Controllers.Incidents.Dtos;

public sealed class CreateIncidentWebRequest
{
    public IFormFile Image { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? CitizenEmail { get; set; }
    public string? AdditionalComment { get; set; }
}
