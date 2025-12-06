namespace Urban.AI.Domain.Users;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users.Events;
using Urban.AI.Domain.Users.ValueObjects;
#endregion

public sealed class User : Entity
{
    #region Constants
    private const bool DefaultIsEmailVerified = false;
    private const bool DefaultIsEnabled = true;

    public const int MaxFirstNameLength = 100;
    public const int MaxLastNameLength = 100;
    #endregion

    private readonly List<Role> _roles = [];

    private User(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        bool isEmailVerified,
        bool isEnabled,
        DateTime createdAt
        ) : base(userId)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsEmailVerified = isEmailVerified;
        CreatedAt = createdAt;
        IsEnabled = isEnabled;
    }


    private User() { }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsEnabled { get; private set; } = true;
    public string IdentityId { get; private set; } = string.Empty;
    public UserDetails? UserDetails { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<Role> Roles => [.. _roles];

    public static User Register(
        string email,
        string firstName,
        string lastName,
        DateTime createdAt
        )
    {
        var user = new User(
            Guid.NewGuid(),
            email,
            firstName,
            lastName,
            DefaultIsEmailVerified,
            DefaultIsEnabled,
            createdAt
        );

        user.AddDomainEvent(new UserRegisterDomainEvent(user.Id));

        user._roles.Add(Role.Registered);

        return user;
    }

    public void CompleteProfile(
        PersonalInfo personalInfo,
        ContactInfo contactInfo)
    {
        UserDetails = new UserDetails(personalInfo, contactInfo);
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }

    public void UpdateBasicInfo(string email, string firstName, string lastName)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    public void UpdateProfilePicture(string pictureUrl)
    {
        if (UserDetails is null) return;

        UserDetails.ContactInfo.UpdatePictureUrl(pictureUrl);
    }
}
