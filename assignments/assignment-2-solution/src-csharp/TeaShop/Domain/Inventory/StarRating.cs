namespace Assignment2Solution.Domain.Inventory;

/// <summary>
///     Represents a star rating for a tea item.
/// </summary>
public record StarRating
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StarRating" /> record.
    /// </summary>
    /// <param name="rating">The star rating value (1-5).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when rating is not between 1 and 5.</exception>
    public StarRating(int rating)
    {
        //invariant: rating must be between 1 and 5
        if (rating is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

        Rating = rating;
    }

    /// <summary>
    ///     Gets the star rating value.
    /// </summary>
    public int Rating { get; }

    /// <summary>
    ///     Returns a string representation of the star rating.
    /// </summary>
    /// <returns>A string with the rating value and a star character.</returns>
    public override string ToString()
    {
        return $"{Rating}{new string('*', Rating)}";
    }
}