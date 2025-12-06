namespace Urban.AI.Domain.Users;

using Urban.AI.Domain.Users.ValueObjects;

public sealed class UserDetails
{
    private UserDetails() { }

    public UserDetails(PersonalInfo personalInfo, ContactInfo contactInfo)
    {
        PersonalInfo = personalInfo;
        ContactInfo = contactInfo;
    }


    public PersonalInfo PersonalInfo { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
}
