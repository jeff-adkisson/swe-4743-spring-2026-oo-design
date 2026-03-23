using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StateMachine.MVC.Contracts;
using StateMachine.MVC.Controllers.Api;
using StateMachine.MVC.Mappers;
using StateMachine.MVC.Services;
using Xunit.Abstractions;

namespace StateMachine.Unit.Tests;

public class EmailFinderControllerTests(ITestOutputHelper console) : TestBase(console)
{
    private readonly IEmailFinderService _emailFinderService = Substitute.For<IEmailFinderService>();
    private readonly ILogger<EmailFinderController> _logger = Substitute.For<ILogger<EmailFinderController>>();
    private EmailFinderController Sut => new(_emailFinderService, _logger);

    [Fact]
    public void HomeController_ShouldFindAddresses_WhenAddressesAreSubmitted()
    {
        // Arrange
        var textToSubmit = SampleTextContainer.GetProjectDescriptionWithValidEmailAddresses();
        var request = new FindEmailAddressesRequest { Text = textToSubmit.Text };
        var expectedResponse = textToSubmit.Matches.ToEmailAddressesResponse();
        _emailFinderService.Find(request.Text).Returns(textToSubmit.Matches);

        // Act
        var response = (OkObjectResult)Sut.FindMatches(request);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        response.StatusCode.Should().Be(200);
        response.Value.As<FindEmailAddressesResponse>().Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public void HomeController_ShouldNotFindAddresses_WhenNoValidAddressesAreSubmitted()
    {
        // Arrange
        var textToSubmit = SampleTextContainer.GetProjectDescriptionWithNoValidEmailAddresses();
        var request = new FindEmailAddressesRequest { Text = textToSubmit.Text };
        var expectedResponse = textToSubmit.Matches.ToEmailAddressesResponse();
        _emailFinderService.Find(request.Text).Returns(textToSubmit.Matches);

        // Act
        var response = (OkObjectResult)Sut.FindMatches(request);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        response.StatusCode.Should().Be(200);
        response.Value.As<FindEmailAddressesResponse>().Should().BeEquivalentTo(expectedResponse);
    }
}
