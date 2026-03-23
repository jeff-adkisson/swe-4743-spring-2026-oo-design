using Xunit.Abstractions;

namespace StateMachine.Unit.Tests;

public abstract class TestBase(ITestOutputHelper console)
{
    protected readonly ITestOutputHelper Console = console;
}
