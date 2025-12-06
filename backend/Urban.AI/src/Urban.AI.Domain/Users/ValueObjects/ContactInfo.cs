namespace Urban.AI.Domain.Users.ValueObjects;

public sealed class ContactInfo
{
    #region Constants
    public const int MaxPictureUrlLength = 500;
    public const int MaxBiographyLength = 500;
    #endregion

    private ContactInfo() { }

    public ContactInfo(PhoneNumber phoneNumber,
                       string? pictureUrl,
                       string? biography)
    {
        PhoneNumber = phoneNumber;
        PictureUrl = pictureUrl;
        Biography = biography;
    }

    public PhoneNumber PhoneNumber { get; private set; }
    public string? PictureUrl { get; private set; }
    public string? Biography { get; private set; }

    public void UpdatePictureUrl(string? pictureUrl)
    {
        PictureUrl = pictureUrl;
    }
}
