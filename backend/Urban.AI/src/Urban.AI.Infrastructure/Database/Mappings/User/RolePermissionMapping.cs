namespace Urban.AI.Infrastructure.Database.Mappings.User;

#region Usings
using Urban.AI.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders; 
#endregion

internal sealed class RolePermissionMapping : IEntityTypeConfiguration<RolePermission>
{
    #region Constants
    private const string TableName = "role_permissions";
    #endregion

    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });

        builder.HasData(
            new RolePermission
            {
                RoleId = Role.Registered.Id,
                PermissionId = Permission.UsersRead.Id
            });
    }
}
