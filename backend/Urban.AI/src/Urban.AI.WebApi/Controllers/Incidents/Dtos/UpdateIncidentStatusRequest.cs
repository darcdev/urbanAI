namespace Urban.AI.WebApi.Controllers.Incidents.Dtos;

#region Usings
using System.ComponentModel.DataAnnotations;
#endregion

public record UpdateIncidentStatusRequest(
    [Required(ErrorMessage = "Status is required")]
    string Status, 
    string? Priority
);
