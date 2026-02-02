using Assignment2Solution.UserInterface;

namespace TeaShop.UnitTest.UserInterface;

public class ApplicationTests
{
    [Fact]
    public void ProcessPurchase_WhenEmptyInput_ShouldContinue()
    {
        // Arrange
        // We need to trigger the loop in Run.
        // Build() will be called first.
        // Then ProcessPurchase will be called if items are found.
        // Then ReadYesNo will be called.

        // To make it easy, we can try to provide input that:
        // 1. Completes QueryBuilder.Build()
        // 2. Provides empty input for ProcessPurchase
        // 3. Provides 'N' for ReadYesNo to exit the loop

        var inputLines = new[]
        {
            "", // QueryBuilder: Name contains (empty)
            "", // QueryBuilder: Is available (empty)
            "", // QueryBuilder: Price min (empty)
            "", // QueryBuilder: Price max (empty)
            "", // QueryBuilder: Rating min (empty)
            "", // QueryBuilder: Rating max (empty)
            "", // QueryBuilder: Sort by Star rating (empty, default D)
            "", // QueryBuilder: Sort by Price (empty, default A)
            "", // Application: Purchase an item? (empty input - SHOULD BE TREATED AS 0)
            "N" // Application: Search for more tea? (N)
        };

        var input = new StringReader(string.Join(Environment.NewLine, inputLines));
        var output = new StringWriter();
        var app = new Application(input, output);

        // Act
        app.Run();

        // Assert
        var outputString = output.ToString();
        // It should contain the new prompt.
        Assert.Contains("or 0 to continue (default): ", outputString);
        // It should not contain an error for the empty input.
        Assert.DoesNotContain("Invalid index. Try again.", outputString);
    }
}