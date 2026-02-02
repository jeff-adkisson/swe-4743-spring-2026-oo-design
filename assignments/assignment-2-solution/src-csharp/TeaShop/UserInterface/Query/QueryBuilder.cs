using System.Globalization;
using Assignment2Solution.Domain.Inventory;
using Assignment2Solution.Domain.Query;
using Assignment2Solution.Domain.Query.Filters;
using Assignment2Solution.Domain.Query.Sorts;

namespace Assignment2Solution.UserInterface.Query;

/// <summary>
///     A builder for creating <see cref="IInventoryQuery" /> objects by prompting the user for input.
/// </summary>
public sealed class QueryBuilder
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly InventoryRepository _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueryBuilder" /> class.
    /// </summary>
    /// <param name="repository">The inventory repository to query.</param>
    /// <param name="input">The text reader for user input.</param>
    /// <param name="output">The text writer for application output.</param>
    public QueryBuilder(InventoryRepository repository, TextReader input, TextWriter output)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    /// <summary>
    ///     Builds a query by prompting the user for search criteria.
    /// </summary>
    /// <returns>A configured inventory query.</returns>
    public IInventoryQuery Build()
    {
        IInventoryQuery query = new AllInventoryQuery(_repository);

        var nameContains = ReadOptionalString("* Tea name contains (leave blank for all names): ");
        query = new NameContainsFilterDecorator(query, nameContains);

        var isAvailable = ReadAvailability();
        query = new AvailabilityFilterDecorator(query, isAvailable);

        var (minPrice, maxPrice) = ReadPriceRange();
        query = new PriceRangeFilterDecorator(query, minPrice, maxPrice);

        var (minRating, maxRating) = ReadStarRatingRange();
        query = new StarRatingRangeFilterDecorator(query, minRating, maxRating);

        var priceSort = ReadSortDirection("* Sort by Price (A/D, default A): ", SortDirection.Ascending);
        query = new SortByPriceDecorator(query, priceSort);

        var starSort = ReadSortDirection("* Sort by Star rating (A/D, default D): ", SortDirection.Descending);
        query = new SortByStarRatingDecorator(query, starSort);

        return query;
    }

    private string? ReadOptionalString(string prompt)
    {
        _output.Write(prompt);
        return _input.ReadLine();
    }

    private bool? ReadAvailability()
    {
        while (true)
        {
            _output.Write("* Is available? (Y/N, default Y): ");
            var input = _input.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return true;

            input = input.Trim();
            if (input.Equals("Y", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                return true;
            if (input.Equals("N", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("No", StringComparison.OrdinalIgnoreCase))
                return false;

            _output.WriteLine("Please enter Y or N.");
        }
    }

    private (decimal Min, decimal Max) ReadPriceRange()
    {
        const decimal defaultMin = 0m;
        const decimal defaultMax = 1000m;

        while (true)
        {
            var min = ReadDecimal("* Price minimum (default $0): ", defaultMin);
            var max = ReadDecimal("* Price maximum (default $1000): ", defaultMax);

            if (min <= max) return (min, max);

            _output.WriteLine("Minimum price cannot be greater than maximum price.");
        }
    }

    private (int Min, int Max) ReadStarRatingRange()
    {
        const int defaultMin = 3;
        const int defaultMax = 5;

        while (true)
        {
            var min = ReadInt("* Star rating minimum (1-5, default 3): ", defaultMin, 1, 5);
            var max = ReadInt("* Star rating maximum (1-5, default 5): ", defaultMax, 1, 5);

            if (min <= max) return (min, max);

            _output.WriteLine("Minimum star rating cannot be greater than maximum star rating.");
        }
    }

    private SortDirection ReadSortDirection(string prompt, SortDirection defaultDirection)
    {
        while (true)
        {
            _output.Write(prompt);
            var input = _input.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return defaultDirection;

            input = input.Trim();
            if (input.Equals("A", StringComparison.OrdinalIgnoreCase)) return SortDirection.Ascending;
            if (input.Equals("D", StringComparison.OrdinalIgnoreCase)) return SortDirection.Descending;

            _output.WriteLine("Please enter A or D.");
        }
    }

    private decimal ReadDecimal(string prompt, decimal defaultValue)
    {
        while (true)
        {
            _output.Write(prompt);
            var input = _input.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return defaultValue;

            var cleaned = input.Trim().Replace("$", string.Empty);
            if (decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.CurrentCulture, out var value) && value >= 0)
                return value;

            _output.WriteLine("Please enter a non-negative number.");
        }
    }

    private int ReadInt(string prompt, int defaultValue, int min, int max)
    {
        while (true)
        {
            _output.Write(prompt);
            var input = _input.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return defaultValue;

            if (int.TryParse(input.Trim(), out var value) && value >= min && value <= max)
                return value;

            _output.WriteLine($"Please enter a whole number between {min} and {max}.");
        }
    }
}