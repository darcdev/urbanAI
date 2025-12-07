namespace Urban.AI.Infrastructure.Database.Mappings.Leader;

#region Usings
using Urban.AI.Domain.Leaders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class LeaderMapping : IEntityTypeConfiguration<Leader>
{
    #region Constants
    private const string TableName = "leaders";
    #endregion

    public void Configure(EntityTypeBuilder<Leader> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(l => l.Id);

        builder.Property(l => l.UserId).IsRequired();
        builder.Property(l => l.DepartmentId).IsRequired();
        builder.Property(l => l.MunicipalityId).IsRequired();
        builder.Property(l => l.Latitude).IsRequired().HasPrecision(18, 8);
        builder.Property(l => l.Longitude).IsRequired().HasPrecision(18, 8);
        builder.Property(l => l.IsActive).IsRequired();
        builder.Property(l => l.CreatedAt).IsRequired();

        builder.HasIndex(l => l.UserId).IsUnique();
        builder.HasIndex(l => l.DepartmentId);
        builder.HasIndex(l => l.MunicipalityId);
        builder.HasIndex(l => l.IsActive);

        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Department)
            .WithMany()
            .HasForeignKey(l => l.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Municipality)
            .WithMany()
            .HasForeignKey(l => l.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
