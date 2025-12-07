namespace Urban.AI.Domain.Incidents;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography;
using Urban.AI.Domain.Leaders;
#endregion

/// <summary>
/// Represents an incident reported by a citizen
/// </summary>
public sealed class Incident : Entity
{
    #region Constants
    public const int RadicateNumberLength = 11;
    public const int CaptionMaxLength = 200;
    public const int DescriptionMaxLength = 1000;
    public const int CommentMaxLength = 500;
    public const int EmailMaxLength = 255;
    public const int ImagePathMaxLength = 500;
    #endregion

    private Incident(
        string radicateNumber,
        string imagePath,
        Location location,
        Guid municipalityId,
        Guid? leaderId,
        string? citizenEmail,
        string? additionalComment) : base(Guid.NewGuid())
    {
        RadicateNumber = radicateNumber;
        ImagePath = imagePath;
        Location = location;
        MunicipalityId = municipalityId;
        LeaderId = leaderId;
        CitizenEmail = citizenEmail;
        AdditionalComment = additionalComment;
        Status = IncidentStatus.Pending;
        Priority = IncidentPriority.NotEstablished;
        CreatedAt = DateTime.UtcNow;
    }

    private Incident() { }

    public string RadicateNumber { get; private set; }
    public string ImagePath { get; private set; }
    public Location Location { get; private set; }
    public string? CitizenEmail { get; private set; }
    public string? AdditionalComment { get; private set; }
    public string? AiDescription { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? SubcategoryId { get; private set; }
    public IncidentStatus Status { get; private set; }
    public IncidentPriority Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? AttentionDate { get; private set; }
    public Guid MunicipalityId { get; private set; }
    public Guid? LeaderId { get; private set; }

    public Municipality Municipality { get; private set; }
    public Leader? Leader { get; private set; }
    public Category? Category { get; private set; }
    public Subcategory? Subcategory { get; private set; }

    public static Incident Create(
        string radicateNumber,
        string imagePath,
        Location location,
        Guid municipalityId,
        Guid? leaderId,
        string? citizenEmail = null,
        string? additionalComment = null)
    {
        return new Incident(
            radicateNumber,
            imagePath,
            location,
            municipalityId,
            leaderId,
            citizenEmail,
            additionalComment);
    }

    public void SetAnalysis(IncidentAnalysis analysis)
    {
        AiDescription = analysis.Description;
        CategoryId = analysis.CategoryId;
        SubcategoryId = analysis.SubcategoryId;
    }

    public void Accept(IncidentPriority priority)
    {
        Status = IncidentStatus.Accepted;
        Priority = priority;
        AttentionDate = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = IncidentStatus.Rejected;
        AttentionDate = DateTime.UtcNow;
    }

    public void UpdatePriority(IncidentPriority priority)
    {
        Priority = priority;
    }
}
