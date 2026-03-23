namespace StateMachine.MVC.Email;

public sealed class StateAdvanceToNextWord : IState
{
    public IState Process(Context context)
    {
        while (!context.IsComplete && !CharacterMatch.IsWordBreak(context.CurrentCharacter))
        {
            context.AdvancePosition();
        }

        return this.GetNextState<StateStartOfWord>();
    }
}
