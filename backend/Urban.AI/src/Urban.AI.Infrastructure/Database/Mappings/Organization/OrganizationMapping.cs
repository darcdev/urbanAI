namespace Urban.AI.Infrastructure.Database.Mappings.Organization;

#region Usings
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class OrganizationMapping : IEntityTypeConfiguration<Domain.Organizations.Organization>
{
    #region Constants
    private const string TableName = "organizations";
    #endregion

    public void Configure(EntityTypeBuilder<Domain.Organizations.Organization> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.OrganizationName).IsRequired().HasMaxLength(Domain.Organizations.Organization.MaxOrganizationNameLength);
        builder.Property(e => e.IsActive).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => e.UserId).IsUnique();
        builder.HasIndex(e => e.OrganizationName);
        builder.HasIndex(e => e.IsActive);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
