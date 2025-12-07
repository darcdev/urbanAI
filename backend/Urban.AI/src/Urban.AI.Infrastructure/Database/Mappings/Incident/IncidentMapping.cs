namespace Urban.AI.Infrastructure.Database.Mappings.Incident;

#region Usings
using Urban.AI.Domain.Incidents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class IncidentMapping : IEntityTypeConfiguration<Incident>
{
    #region Constants
    private const string TableName = "incidents";
    #endregion

    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.RadicateNumber)
            .IsRequired()
            .HasMaxLength(Incident.RadicateNumberLength);

        builder.Property(i => i.ImagePath)
            .IsRequired()
            .HasMaxLength(Incident.ImagePathMaxLength);

        builder.OwnsOne(i => i.Location, location =>
        {
            location.Property(l => l.Latitude)
                .IsRequired()
                .HasPrecision(18, 8);

            location.Property(l => l.Longitude)
                .IsRequired()
                .HasPrecision(18, 8);
        });

        builder.Property(i => i.CitizenEmail)
            .HasMaxLength(Incident.EmailMaxLength);

        builder.Property(i => i.AdditionalComment)
            .HasMaxLength(Incident.CommentMaxLength);

        builder.Property(i => i.AiDescription)
            .HasMaxLength(Incident.DescriptionMaxLength);

        builder.Property(i => i.CategoryId);

        builder.Property(i => i.SubcategoryId);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(i => i.Priority)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(i => i.CreatedAt).IsRequired();

        builder.Property(i => i.AttentionDate);

        builder.Property(i => i.MunicipalityId).IsRequired();

        builder.Property(i => i.LeaderId);

        builder.HasIndex(i => i.RadicateNumber).IsUnique();
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.Priority);
        builder.HasIndex(i => i.MunicipalityId);
        builder.HasIndex(i => i.LeaderId);
        builder.HasIndex(i => i.CreatedAt);

        builder.HasOne(i => i.Municipality)
            .WithMany()
            .HasForeignKey(i => i.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Leader)
            .WithMany()
            .HasForeignKey(i => i.LeaderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Subcategory)
            .WithMany()
            .HasForeignKey(i => i.SubcategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
