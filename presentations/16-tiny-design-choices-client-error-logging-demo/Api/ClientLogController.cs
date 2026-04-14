using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api;

[ApiController]
[Route("api/client-log")]
public class ClientLogController : ControllerBase
{
    private readonly ILogger<ClientLogController> _logger;

    public ClientLogController(ILogger<ClientLogController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [EnableRateLimiting("client-log")]
    public IActionResult Post([FromBody] ClientErrorReport report)
    {
        _logger.LogError(
            "[CLIENT-SIDE ERROR] Message={Message} | Url={Url} | User={User} | " +
            "Component={Component} | Timestamp={Timestamp} | UserAgent={UserAgent} | Stack={Stack}",
            report.Message,
            report.Url,
            report.User ?? "(anonymous)",
            report.Component ?? "(unknown)",
            report.Timestamp,
            report.UserAgent,
            report.Stack ?? "(no stack trace)");

        return NoContent();
    }
}
