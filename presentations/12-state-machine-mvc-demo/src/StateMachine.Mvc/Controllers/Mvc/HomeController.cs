using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StateMachine.MVC.Models;
using StateMachine.MVC.Services;

namespace StateMachine.MVC.Controllers.Mvc;

public class HomeController(
    IEmailFinderService emailFinderService,
    ILogger<HomeController> logger) : Controller
{
    public IActionResult Index() => View(new FindEmailAddressesModel());

    [HttpPost]
    public IActionResult Index(FindEmailAddressesModel model)
    {
        logger.Log(LogLevel.Debug, "POST Received to {Controller} {Name} method", nameof(HomeController), nameof(Index));

        logger.Log(LogLevel.Debug, "Text to scan: {TextToScan}", model.Text);
        var matches = emailFinderService.Find(model.Text).ToList();

        model.Matches = matches;
        logger.Log(LogLevel.Debug, "Matches: {Matches}", matches);

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
