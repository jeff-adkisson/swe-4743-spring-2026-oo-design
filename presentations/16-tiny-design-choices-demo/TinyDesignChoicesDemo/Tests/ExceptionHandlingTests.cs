using System.Diagnostics;
using Xunit.Abstractions;

namespace TinyDesignChoicesDemo.Tests;

public class ExceptionHandlingTests
{
    private readonly ITestOutputHelper _out;

    public ExceptionHandlingTests(ITestOutputHelper output) => _out = output;

    // --- Anti-Pattern: Exceptions for control flow ---

    [Fact]
    public void Bad_UsingExceptionsForControlFlow()
    {
        int ParseAgeBad(string input)
        {
            try { return int.Parse(input); }
            catch (FormatException) { return -1; }
        }

        // Time the exception-based approach on many bad inputs
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 10_000; i++) ParseAgeBad("not-a-number");
        sw.Stop();

        _out.WriteLine("❌ Anti-pattern: using exceptions as a control-flow mechanism.");
        _out.WriteLine($"  ParseAgeBad(\"not-a-number\") throws + catches 10,000 times in {sw.ElapsedMilliseconds} ms.");
        _out.WriteLine("  Every throw captures a stack trace — that's the cost you pay for using exceptions to branch.");
        _out.WriteLine($"  Result: {ParseAgeBad("not-a-number")}");

        Assert.Equal(-1, ParseAgeBad("not-a-number"));
    }

    [Fact]
    public void Good_UsingTryParse()
    {
        int ParseAgeGood(string input)
            => int.TryParse(input, out var age) ? age : -1;

        // Time the TryParse approach on the same bad inputs
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 10_000; i++) ParseAgeGood("not-a-number");
        sw.Stop();

        _out.WriteLine("✅ Correct: use TryParse for expected invalid input.");
        _out.WriteLine($"  ParseAgeGood(\"not-a-number\") called 10,000 times in {sw.ElapsedMilliseconds} ms.");
        _out.WriteLine("  No exceptions thrown — branch on a bool instead.");
        _out.WriteLine($"  ParseAgeGood(\"not-a-number\") = {ParseAgeGood("not-a-number")}");
        _out.WriteLine($"  ParseAgeGood(\"25\")           = {ParseAgeGood("25")}");

        Assert.Equal(-1, ParseAgeGood("not-a-number"));
        Assert.Equal(25, ParseAgeGood("25"));
    }

    // --- Preserving stack traces ---

    [Fact]
    public void Bad_LosingStackTrace()
    {
        Action act = () =>
        {
            try { throw new InvalidOperationException("Original error"); }
            catch (Exception e)
            {
                throw new ApplicationException("Wrapped: " + e.Message);
            }
        };

        var ex = Assert.Throws<ApplicationException>(act);

        _out.WriteLine("❌ Anti-pattern: wrapping an exception without passing it as inner.");
        _out.WriteLine($"  Outer:           {ex.GetType().Name}: {ex.Message}");
        _out.WriteLine($"  InnerException:  {(ex.InnerException?.ToString() ?? "(null — original trace LOST)")}");
        _out.WriteLine("  The debugger now shows only the wrapper's stack — original failure location is gone.");

        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void Good_PreservingStackTrace()
    {
        Action act = () =>
        {
            try { throw new InvalidOperationException("Original error"); }
            catch (Exception e)
            {
                throw new ApplicationException("Wrapped", e);
            }
        };

        var ex = Assert.Throws<ApplicationException>(act);

        _out.WriteLine("✅ Correct: pass the original exception as the inner exception.");
        _out.WriteLine($"  Outer:          {ex.GetType().Name}: {ex.Message}");
        _out.WriteLine($"  InnerException: {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
        _out.WriteLine("  Both stack traces are preserved — the debugger can walk from wrapper to root cause.");

        Assert.NotNull(ex.InnerException);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Equal("Original error", ex.InnerException!.Message);
    }

    // --- Custom exceptions carry domain context ---

    [Fact]
    public void CustomException_CarriesDomainContext()
    {
        Action act = () =>
            throw new OrderProcessingException("ORD-42", "Payment gateway timeout");

        var ex = Assert.Throws<OrderProcessingException>(act);

        _out.WriteLine("Custom exceptions carry structured domain data the caller can act on.");
        _out.WriteLine($"  ex.Message = \"{ex.Message}\"");
        _out.WriteLine($"  ex.OrderId = \"{ex.OrderId}\"   ← no string parsing required");
        _out.WriteLine("A generic Exception would force the caller to parse the message — fragile and error-prone.");

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

        _out.WriteLine("Custom exceptions should still preserve the cause when wrapping.");
        _out.WriteLine($"  Outer: {ex.GetType().Name}: \"{ex.Message}\" (OrderId={ex.OrderId})");
        _out.WriteLine($"  Inner: {ex.InnerException!.GetType().Name}: \"{ex.InnerException.Message}\"");

        Assert.NotNull(ex.InnerException);
        Assert.IsType<TimeoutException>(ex.InnerException);
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
