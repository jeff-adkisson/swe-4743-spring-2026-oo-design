namespace StateMachine.MVC.Email;

public sealed class StateCaptureTopLevelDomain : IState
{
    public IState Process(Context context)
    {
        var length = 0;
        while (CharacterMatch.IsAlphanumeric(context.CurrentCharacter))
        {
            context.AdvancePosition();
            length++;
        }

        return length is >= 2 and <= 10
            ? this.GetNextState<StateAddEmailAddress>()
            : this.GetNextState<StateAdvanceToNextWord>();
    }
}
