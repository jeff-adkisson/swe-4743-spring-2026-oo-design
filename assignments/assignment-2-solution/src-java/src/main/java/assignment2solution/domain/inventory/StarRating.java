package assignment2solution.domain.inventory;

/**
 * Represents a star rating for a tea item.
 */
public final class StarRating {
    private final int rating;

    /**
     * Initializes a new instance of the {@link StarRating} class.
     *
     * @param rating The star rating value (1-5).
     * @throws IllegalArgumentException when rating is not between 1 and 5.
     */
    public StarRating(int rating) {
        if (rating < 1 || rating > 5) {
            throw new IllegalArgumentException("Rating must be between 1 and 5");
        }
        this.rating = rating;
    }

    /**
     * Gets the star rating value.
     */
    public int getRating() {
        return rating;
    }

    /**
     * Returns a string representation of the star rating.
     *
     * @return A string with the rating value and a star character.
     */
    @Override
    public String toString() {
        return rating + "*".repeat(rating);
    }
}
