using System.Diagnostics;

namespace StateMachine.MVC.Email;

public sealed class StateComplete : IState
{
    public IState Process(Context context)
    {
        Debug.Assert(context.IsComplete);
        return this;
    }
}
