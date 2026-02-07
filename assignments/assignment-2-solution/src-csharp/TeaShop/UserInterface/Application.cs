using Assignment2Solution.Domain.Inventory;
using Assignment2Solution.Domain.Query;
using Assignment2Solution.UserInterface.PaymentMethodGenerator;
using Assignment2Solution.UserInterface.Query;

namespace Assignment2Solution.UserInterface;

/// <summary>
///     The main application class that coordinates the user interface and domain logic.
/// </summary>
public sealed class Application
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly IReadOnlyList<IPaymentStrategyGenerator> _paymentMethods;

    private readonly QueryBuilder _queryBuilder;
    private readonly QueryOutputWriter _queryOutputWriter;
    private readonly InventoryRepository _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Application" /> class.
    /// </summary>
    /// <param name="input">The text reader for user input.</param>
    /// <param name="output">The text writer for application output.</param>
    public Application(TextReader input, TextWriter output)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));

        _repository = new InventoryRepository();
        _queryBuilder = new QueryBuilder(_repository, _input, _output);
        _queryOutputWriter = new QueryOutputWriter(_output);

        _paymentMethods = PaymentStrategyGeneratorListFactory.Get();
    }

    /// <summary>
    ///     Runs the application loop.
    /// </summary>
    public void Run()
    {
        DisplayWelcomeMessage();

        while (true)
        {
            var query = _queryBuilder.Build();
            var output = QueryOutput.From(query);
            _queryOutputWriter.Write(output);
            _output.WriteLine();

            if (output.Items.Count == 0)
                _output.WriteLine("*** Nothing matched your search ***");
            else
                ProcessPurchase(output);

            _output.WriteLine();
            var searchForMore = ReadYesNo("Search for more tea? (Y/N, default Y): ", true);
            if (!searchForMore) break;
            _output.WriteLine();
        }
    }

    private void DisplayWelcomeMessage()
    {
        _output.WriteLine("WELCOME TO JEFF'S TEA SHOP");
        _output.WriteLine();
        _output.WriteLine("Complete the prompts to search our selection of fine teas.");
        _output.WriteLine();
    }

    private void ProcessPurchase(QueryOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (output.Items.Count == 0)
        {
            _output.WriteLine("!!! No items available to purchase.");
            return;
        }

        while (true)
        {
            var itemSelectionPrompt = $"1-{output.Items.Count} or 0 to continue (default)";
            _output.Write($"Purchase an item? Enter item number {itemSelectionPrompt}: ");
            var input = _input.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return;

            if (!int.TryParse(input.Trim(), out var index) || index < 0 || index > output.Items.Count)
            {
                _output.WriteLine("!!! Invalid item number. Try again.");
                continue;
            }

            if (index == 0) return;

            var selected = output.Items[index - 1];

            if (selected.Quantity <= 0)
            {
                _output.WriteLine($"!!! Sorry, there is inventory available for {selected.Name}.");
                return;
            }

            _output.Write($"Quantity for \"{selected.Name}\" (1-{selected.Quantity}): ");
            var qtyInput = _input.ReadLine();
            if (!int.TryParse(qtyInput?.Trim(), out var quantity) || quantity < 1 || quantity > selected.Quantity)
            {
                _output.WriteLine("!!! Invalid quantity.");
                continue;
            }

            var totalPrice = selected.Price * quantity;
            _output.WriteLine($"*** Total Price: {totalPrice:C}");

            ProcessCheckout(selected, quantity);
            break;
        }
    }

    private void ProcessCheckout(QueriedInventoryItem item, int quantity)
    {
        _output.WriteLine("*** Choose a payment method:");
        for (var i = 0; i < _paymentMethods.Count; i++) _output.WriteLine($"{i + 1}. {_paymentMethods[i].Name}");

        _output.Write("Selection: ");

        var choice = _input.ReadLine();
        if (!int.TryParse(choice, out var index) || index < 1 || index > _paymentMethods.Count)
        {
            _output.WriteLine("!!! Invalid payment method selection. Checkout cancelled.");
            return;
        }

        var strategy = _paymentMethods[index - 1].CreateStrategy(_input, _output);
        strategy.Checkout(item, quantity, _output);

        // Decrease inventory quantity (using negative value for decrease)
        _repository.UpdateQuantity(item.InventoryItemId, -1 * quantity);

        var purchaseDesc = $"{quantity} packages of {item.Name}";
        _output.WriteLine($"*** Purchase complete. Your {purchaseDesc} is on the way ***");
    }

    private bool ReadYesNo(string prompt, bool defaultValue)
    {
        while (true)
        {
            _output.Write(prompt);
            var input = _input.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return defaultValue;

            if (input.Trim().Equals("Y", StringComparison.OrdinalIgnoreCase))
                return true;
            if (input.Trim().Equals("N", StringComparison.OrdinalIgnoreCase))
                return false;

            _output.WriteLine("Please enter Y or N.");
        }
    }
}