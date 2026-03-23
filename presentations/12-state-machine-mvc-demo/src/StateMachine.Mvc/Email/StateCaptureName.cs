namespace StateMachine.MVC.Email;

public sealed class StateCaptureName : IState
{
    public IState Process(Context context)
    {
        context.SetCurrentStartIndex();

        var length = 0;
        while (CharacterMatch.IsAlphanumericOrSymbol(context.CurrentCharacter))
        {
            context.AdvancePosition();
            length++;
        }

        if (length <= 0 || !CharacterMatch.IsAmpersand(context.CurrentCharacter))
            return this.GetNextState<StateAdvanceToNextWord>();

        context.AdvancePosition();
        return this.GetNextState<StateCaptureDomain>();
    }
}
