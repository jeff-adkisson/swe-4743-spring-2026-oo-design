namespace Assignment2Solution.UserInterface.Query;

/// <summary>
///     A writer for displaying query results to the user.
/// </summary>
public sealed class QueryOutputWriter
{
    private readonly TextWriter _output;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QueryOutputWriter" /> class.
    /// </summary>
    /// <param name="output">The text writer for application output.</param>
    public QueryOutputWriter(TextWriter output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    /// <summary>
    ///     Writes the query output to the configured text writer.
    /// </summary>
    /// <param name="output">The query output to write.</param>
    /// <exception cref="ArgumentNullException">Thrown when output is null.</exception>
    public void Write(QueryOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        _output.WriteLine("");
        _output.WriteLine("Applied Filters and Sorts:");
        if (output.AppliedFiltersAndSorts.Count == 0)
            _output.WriteLine("- (none)");
        else
            foreach (var d in output.AppliedFiltersAndSorts)
                _output.WriteLine($"- {d}");

        _output.WriteLine();
        _output.WriteLine($"{output.Items.Count} items matched your query:");

        foreach (var i in output.Items)
        {
            var quantityAvailable = i.Quantity == 0
                ? "(OUT OF STOCK)"
                : $"Qty: {i.Quantity,-4 }";
            _output.WriteLine($"{i.Index,2}. {i.Name,-20}  " +
                              $"{i.Price,6:C}  " +
                              $"{quantityAvailable}  " +
                              $"{i.StarRating}");
        }
    }
}