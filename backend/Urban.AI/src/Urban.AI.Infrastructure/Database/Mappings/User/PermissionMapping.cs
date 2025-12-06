namespace Urban.AI.Infrastructure.Database.Mappings.User;

using Urban.AI.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class PermissionMapping : IEntityTypeConfiguration<Permission>
{
    #region Constants
    private const string TableName = "permissions";
    #endregion

    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(permission => permission.Id);

        builder.HasData(Permission.UsersRead);
    }
}
