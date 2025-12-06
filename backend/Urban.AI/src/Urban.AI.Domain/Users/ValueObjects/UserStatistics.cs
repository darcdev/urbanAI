namespace Urban.AI.Domain.Users.ValueObjects;

public sealed class UserStatistics(int ridesTaken,
                                   int ridesOffered,
                                   float avgDriverRating,
                                   int totalDriverRatings)
{
    public int RidesTaken { get; private set; } = ridesTaken;

    public int RidesOffered { get; private set; } = ridesOffered;

    public float AvgDriverRating { get; private set; } = avgDriverRating;

    public int TotalDriverRatings { get; private set; } = totalDriverRatings;

    public void IncrementRidesTaken() => RidesTaken++;

    public void IncrementRidesOffered() => RidesOffered++;

    public void UpdateDriverRating(float newRating)
    {
        TotalDriverRatings++;
        AvgDriverRating = (AvgDriverRating * (TotalDriverRatings - 1) + newRating) / TotalDriverRatings;
    }
}