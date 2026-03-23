namespace StateMachine.MVC.Email;

public static class NextState
{
    private static readonly Dictionary<Type, IState> States = new()
    {
        [typeof(StateAddEmailAddress)] = new StateAddEmailAddress(),
        [typeof(StateAdvanceToNextWord)] = new StateAdvanceToNextWord(),
        [typeof(StateCaptureDomain)] = new StateCaptureDomain(),
        [typeof(StateCaptureName)] = new StateCaptureName(),
        [typeof(StateCaptureTopLevelDomain)] = new StateCaptureTopLevelDomain(),
        [typeof(StateStartOfWord)] = new StateStartOfWord(),
        [typeof(StateComplete)] = new StateComplete()
    };

    /// <summary>
    ///     Accepts type T and returns the corresponding IState
    /// </summary>
    public static IState GetNextState<T>(this IState _) where T : IState =>
        States[typeof(T)];

    /// <summary>
    /// Returns the starting state
    /// </summary>
    public static IState Start() => States[typeof(StateStartOfWord)];

    /// <summary>
    /// Returns true if <see cref="StateComplete"/> has been reached
    /// </summary>
    public static bool IsComplete(this IState state) =>
        state is StateComplete;
}