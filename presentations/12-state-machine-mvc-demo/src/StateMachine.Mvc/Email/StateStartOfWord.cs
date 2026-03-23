namespace StateMachine.MVC.Email;

public sealed class StateStartOfWord : IState
{
    public IState Process(Context context)
    {
        while (!context.IsComplete)
        {
            if (CharacterMatch.IsAlphanumericOrSymbol(context.CurrentCharacter))
                return this.GetNextState<StateCaptureName>();

            context.AdvancePosition();
        }

        return this.GetNextState<StateComplete>();
    }
}
