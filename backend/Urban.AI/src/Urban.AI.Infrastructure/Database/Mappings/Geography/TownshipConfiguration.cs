namespace Urban.AI.Infrastructure.Database.Mappings.Geography;

#region Usings
using Urban.AI.Domain.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class TownshipConfiguration : IEntityTypeConfiguration<Township>
{
    #region Constants
    private const string TownshipTableName = "townships";
    private const int NameMaxLength = 100;
    private const int TownshipDaneCodeMaxLength = 15;
    private const int MunicipalityDaneCodeMaxLength = 10;
    #endregion

    public void Configure(EntityTypeBuilder<Township> builder)
    {
        builder.ToTable(TownshipTableName);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TownshipDaneCode).IsRequired().HasMaxLength(TownshipDaneCodeMaxLength);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(NameMaxLength);
        builder.Property(t => t.MunicipalityDaneCode).IsRequired().HasMaxLength(MunicipalityDaneCodeMaxLength);

        builder.HasIndex(t => t.TownshipDaneCode).IsUnique();
        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.MunicipalityDaneCode);
    }
}