using Microsoft.AspNetCore.Mvc;
using StateMachine.MVC.Contracts;
using StateMachine.MVC.Services;

namespace StateMachine.MVC.Controllers.Api;

[ApiController]
public class EmailFinderController(
    IEmailFinderService emailFinderService,
    ILogger<EmailFinderController> logger) : Controller
{
    [HttpPost("api/email/find")]
    public IActionResult FindMatches([FromBody] FindEmailAddressesRequest request)
    {
        logger.Log(LogLevel.Debug, "POST received to API {Controller} {Name} method", nameof(EmailFinderController), nameof(FindMatches));

        logger.Log(LogLevel.Debug, "Text to scan: {TextToScan}", request.Text);
        var matches = emailFinderService.Find(request.Text);

        logger.Log(LogLevel.Debug, "Matches: {Matches}", matches);
        var response = new FindEmailAddressesResponse { Matches = matches };
        return Ok(response);
    }
}
