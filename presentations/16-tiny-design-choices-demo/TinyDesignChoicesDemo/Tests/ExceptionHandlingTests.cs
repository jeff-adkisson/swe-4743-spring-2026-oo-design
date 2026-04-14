namespace TinyDesignChoicesDemo.Tests;

public class ExceptionHandlingTests
{
    // --- Anti-Pattern: Exceptions for control flow ---

    [Fact]
    public void Bad_UsingExceptionsForControlFlow()
    {
        // ❌ Anti-pattern: using exceptions to parse — expensive and unclear
        int ParseAgeBad(string input)
        {
            try { return int.Parse(input); }
            catch (FormatException) { return -1; }
        }

        Assert.Equal(-1, ParseAgeBad("not-a-number"));
    }

    [Fact]
    public void Good_UsingTryParse()
    {
        // ✅ Correct: use TryParse for expected invalid input
        int ParseAgeGood(string input)
        {
            return int.TryParse(input, out var age) ? age : -1;
        }

        Assert.Equal(-1, ParseAgeGood("not-a-number"));
        Assert.Equal(25, ParseAgeGood("25"));
    }

    // --- Preserving stack traces ---

    [Fact]
    public void Bad_LosingStackTrace()
    {
        // ❌ Anti-pattern: wrapping without inner exception loses the original stack trace
        Action act = () =>
        {
            try { throw new InvalidOperationException("Original error"); }
            catch (Exception e)
            {
                throw new ApplicationException("Wrapped: " + e.Message);
            }
        };

        var ex = Assert.Throws<ApplicationException>(act);
        // The inner exception is null — original stack trace is lost
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Good_PreservingStackTrace()
    {
        // ✅ Correct: pass original exception as inner exception
        Action act = () =>
        {
            try { throw new InvalidOperationException("Original error"); }
            catch (Exception e)
            {
                throw new ApplicationException("Wrapped", e);
            }
        };

        var ex = Assert.Throws<ApplicationException>(act);
        // The original exception is preserved
        Assert.NotNull(ex.InnerException);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Equal("Original error", ex.InnerException.Message);
    }

    // --- Custom exceptions carry domain context ---

    [Fact]
    public void CustomException_CarriesDomainContext()
    {
        Action act = () =>
            throw new OrderProcessingException("ORD-42", "Payment gateway timeout");

        var ex = Assert.Throws<OrderProcessingException>(act);
        Assert.Equal("ORD-42", ex.OrderId);
        Assert.Equal("Payment gateway timeout", ex.Message);
    }

    [Fact]
    public void CustomException_PreservesInnerException()
    {
        var inner = new TimeoutException("Gateway did not respond");

        Action act = () =>
            throw new OrderProcessingException("ORD-42", "Payment failed", inner);

        var ex = Assert.Throws<OrderProcessingException>(act);
        Assert.NotNull(ex.InnerException);
        Assert.IsType<TimeoutException>(ex.InnerException);
    }

    // --- Result type for expected failures ---

    [Fact]
    public void Result_SuccessPath()
    {
        var result = CreateOrder(itemCount: 3);

        Assert.True(result.IsSuccess);
        Assert.Equal("order-123", result.Value);
    }

    [Fact]
    public void Result_FailurePath()
    {
        var result = CreateOrder(itemCount: 0);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cart is empty", result.Error);
    }

    // --- Helpers ---

    private static Result<string> CreateOrder(int itemCount)
    {
        if (itemCount == 0)
            return Result<string>.Fail("Cart is empty");
        return Result<string>.Ok("order-123");
    }
}

public class OrderProcessingException : Exception
{
    public string OrderId { get; }

    public OrderProcessingException(string orderId, string message, Exception? inner = null)
        : base(message, inner)
    {
        OrderId = orderId;
    }
}

public record Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error is null;

    private Result(T? value, string? error) => (Value, Error) = (value, error);

    public static Result<T> Ok(T value) => new(value, null);
    public static Result<T> Fail(string error) => new(default, error);
}
