namespace Urban.AI.Infrastructure.Database.Mappings.Incident;

#region Usings
using Urban.AI.Domain.Incidents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class CategoryMapping : IEntityTypeConfiguration<Category>
{
    #region Constants
    private const string TableName = "categories";
    #endregion

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(Category.CodeMaxLength);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(Category.NameMaxLength);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(Category.DescriptionMaxLength);

        builder.HasIndex(c => c.Code).IsUnique();

        builder.HasMany(c => c.Subcategories)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
