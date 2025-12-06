namespace Urban.AI.Infrastructure.Database.Mappings.User;

#region Usings
using Urban.AI.Domain.Users;
using Urban.AI.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#endregion

internal sealed class UserMapping : IEntityTypeConfiguration<User>
{
    #region Constants
    private const string TableName = "users";
    private const int MaxEmailLength = 150;
    private const int MaxPhoneNumberLength = 15;
    private const int MaxGenderLength = 30;
    private const int MaxDocumentTypeLength = 40;
    #endregion

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(user => user.Id);

        builder.Property(pi => pi.FirstName).HasMaxLength(User.MaxFirstNameLength);
        builder.Property(pi => pi.LastName).HasMaxLength(User.MaxLastNameLength);
        builder.Property(user => user.Email).HasMaxLength(MaxEmailLength);
        builder.Property(user => user.IsEmailVerified);
        builder.Property(user => user.IsEnabled);
        builder.Property(user => user.CreatedAt);

        #region UserDetails Configuration
        builder.OwnsOne(user => user.UserDetails, userDetails =>
        {
            userDetails.ToTable("user_details");

            #region PersonalInfo Configuration
            userDetails.OwnsOne(d => d.PersonalInfo, personalInfo =>
            {
                personalInfo.Property(p => p.BirthDate);

                personalInfo.Property(p => p.Gender)
                    .HasConversion(g => g.Value, v => Gender.Create(v))
                    .HasMaxLength(MaxGenderLength);

                personalInfo.Property(p => p.DocumentType)
                    .HasConversion(doc => doc.Value, v => DocumentType.Create(v))
                    .HasMaxLength(MaxDocumentTypeLength);

                personalInfo.Property(p => p.DocumentNumber)
                    .HasMaxLength(PersonalInfo.MaxDocumentNumberLength);
            });
            #endregion

            #region ContactInfo Configuration
            userDetails.OwnsOne(d => d.ContactInfo, ci =>
            {
                ci.Property(p => p.PhoneNumber)
                    .HasConversion(phone => phone.Value, v => PhoneNumber.Create(v))
                    .HasMaxLength(MaxPhoneNumberLength);

                ci.Property(p => p.PictureUrl)
                    .IsRequired(false)
                    .HasMaxLength(ContactInfo.MaxPictureUrlLength);

                ci.Property(p => p.Biography)
                    .IsRequired(false)
                    .HasMaxLength(ContactInfo.MaxBiographyLength);
            });
            #endregion
        });

        builder.Navigation(u => u.UserDetails).IsRequired(false);
        #endregion

        builder.HasIndex(user => user.IdentityId).IsUnique();
        builder.HasIndex(user => user.Email).IsUnique();
    }
}