namespace Urban.AI.Domain.Incidents;

public sealed class Location
{
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    private Location(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Location Create(decimal latitude, decimal longitude)
    {
        return new Location(latitude, longitude);
    }
}
