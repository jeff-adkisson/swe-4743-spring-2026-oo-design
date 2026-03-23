using StateMachine.MVC.Email;

namespace StateMachine.MVC.Services;

public class EmailFinderService : IEmailFinderService
{
    public Match[] Find(string? text)
    {
        return Finder.Find(text);
    }
}
