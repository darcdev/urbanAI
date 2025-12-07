namespace Urban.AI.Infrastructure.Database.Mappings.Incident;

#region Usings
using Urban.AI.Domain.Incidents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class SubcategoryMapping : IEntityTypeConfiguration<Subcategory>
{
    #region Constants
    private const string TableName = "subcategories";
    #endregion

    public void Configure(EntityTypeBuilder<Subcategory> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(Subcategory.NameMaxLength);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(Subcategory.DescriptionMaxLength);

        builder.Property(s => s.CategoryId).IsRequired();

        builder.HasIndex(s => s.CategoryId);
        builder.HasIndex(s => new { s.CategoryId, s.Name }).IsUnique();
    }
}
