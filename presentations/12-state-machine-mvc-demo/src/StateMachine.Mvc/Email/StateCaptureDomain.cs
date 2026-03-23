namespace StateMachine.MVC.Email;

public sealed class StateCaptureDomain : IState
{
    public IState Process(Context context)
    {
        var length = 0;
        while (CharacterMatch.IsAlphanumeric(context.CurrentCharacter))
        {
            context.AdvancePosition();
            length++;
        }

        if (length == 0 || !CharacterMatch.IsDot(context.CurrentCharacter))
            return this.GetNextState<StateAdvanceToNextWord>();

        context.AdvancePosition();
        return this.GetNextState<StateCaptureTopLevelDomain>();
    }
}
