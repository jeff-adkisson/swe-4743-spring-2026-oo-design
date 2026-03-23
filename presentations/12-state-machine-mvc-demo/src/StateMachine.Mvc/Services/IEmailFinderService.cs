using StateMachine.MVC.Email;

namespace StateMachine.MVC.Services;

public interface IEmailFinderService
{
    Match[] Find(string? text);
}
