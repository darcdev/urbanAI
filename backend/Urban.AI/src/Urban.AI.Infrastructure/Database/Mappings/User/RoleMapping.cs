namespace Urban.AI.Infrastructure.Database.Mappings.User;

#region Usings
using Urban.AI.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class RoleMapping : IEntityTypeConfiguration<Role>
{
    #region Constants
    private const string TableName = "roles";
    #endregion

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(role => role.Id);

        builder.HasMany(role => role.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>();

        builder.HasData(Role.Registered);
    }
}
