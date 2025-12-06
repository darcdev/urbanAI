namespace Urban.AI.Infrastructure.Database.Mappings.Geography;

#region Usings
using Urban.AI.Domain.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class MunicipalityConfiguration : IEntityTypeConfiguration<Municipality>
{
    #region Constants
    private const string MunicipalityTableName = "municipalities";
    private const int NameMaxLength = 100;
    private const int MunicipalityDaneCodeMaxLength = 10;
    private const int DepartmentDaneCodeMaxLength = 10;
    #endregion

    public void Configure(EntityTypeBuilder<Municipality> builder)
    {
        builder.ToTable(MunicipalityTableName);

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MunicipalityDaneCode).IsRequired().HasMaxLength(MunicipalityDaneCodeMaxLength);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(NameMaxLength);
        builder.Property(m => m.DepartmentDaneCode).IsRequired().HasMaxLength(DepartmentDaneCodeMaxLength);
        builder.Property(m => m.Latitude).HasPrecision(18, 8);
        builder.Property(m => m.Longitude).HasPrecision(18, 8);

        builder.HasIndex(m => m.MunicipalityDaneCode).IsUnique();
        builder.HasIndex(m => m.Name);
        builder.HasIndex(m => m.DepartmentDaneCode);

        builder.HasMany<Township>()
            .WithOne()
            .HasPrincipalKey(m => m.MunicipalityDaneCode)
            .HasForeignKey(t => t.MunicipalityDaneCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}