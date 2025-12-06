namespace Urban.AI.Infrastructure.Database.Mappings.Geography;

#region Usings
using Urban.AI.Domain.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    #region Constants
    private const string DepartmentTableName = "departments";
    private const int NameMaxLength = 100;
    private const int DepartmentDaneCodeMaxLength = 10;
    #endregion

    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable(DepartmentTableName);

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DepartmentDaneCode).IsRequired().HasMaxLength(DepartmentDaneCodeMaxLength);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(NameMaxLength);
        builder.Property(d => d.Latitude).HasPrecision(18, 8);
        builder.Property(d => d.Longitude).HasPrecision(18, 8);

        builder.HasIndex(d => d.DepartmentDaneCode).IsUnique();
        builder.HasIndex(d => d.Name);

        builder.HasMany<Municipality>()
            .WithOne()
            .HasPrincipalKey(d => d.DepartmentDaneCode)
            .HasForeignKey(m => m.DepartmentDaneCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}