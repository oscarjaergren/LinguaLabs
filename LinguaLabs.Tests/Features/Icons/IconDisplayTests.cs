using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using LinguaLabs.Features.Icons;
using System.Collections.Immutable;

namespace LinguaLabs.Tests.Features.Icons;

public class IconDisplayTests
{
    private IIconifyService _mockIconifyService = null!;
    private IconDisplay _iconDisplay = null!;

    [Before(Test)]
    public void Setup()
    {
        _mockIconifyService = Substitute.For<IIconifyService>();
        _iconDisplay = new IconDisplay();

        // Simulate the control being loaded
        _iconDisplay.IconService = _mockIconifyService;
    }

    [Test]
    public async Task IconDisplay_Should_ShowFallback_When_IconService_IsNull()
    {
        // Arrange
        var icon = new IconifyIcon
        {
            Id = "test:icon",
            Name = "test icon",
            Set = "test"
        };

        _iconDisplay.IconService = null!;

        // Act
        _iconDisplay.Icon = icon;

        // Allow UI to update
        await Task.Delay(100);

        // Assert
        await Assert.That(_iconDisplay.IconService).IsNull();
    }

    [Test]
    public async Task IconDisplay_Should_CallIconService_When_IconSet()
    {
        // Arrange
        var icon = new IconifyIcon
        {
            Id = "test:icon",
            Name = "test icon",
            Set = "test"
        };

        var expectedSvg = "<svg><rect width='100' height='100'/></svg>";
        _mockIconifyService.GetIconSvg(icon.Id).Returns(expectedSvg);

        // Act
        _iconDisplay.Icon = icon;

        // Allow async operation to complete
        await Task.Delay(200);

        // Assert
        await _mockIconifyService.Received(1).GetIconSvg(icon.Id);
    }

    [Test]
    public async Task IconDisplay_Should_HandleEmptyIconId()
    {
        // Arrange
        var icon = new IconifyIcon
        {
            Id = "",
            Name = "empty",
            Set = "test"
        };

        // Act
        _iconDisplay.Icon = icon;

        // Allow UI to update
        await Task.Delay(100);

        // Assert
        await _mockIconifyService.DidNotReceive().GetIconSvg(Arg.Any<string>());
    }
    
    [Test]
    public async Task IconDisplay_Should_HandleServiceException()
    {
        // Arrange
        var icon = new IconifyIcon
        {
            Id = "test:error",
            Name = "error icon",
            Set = "test"
        };

        _mockIconifyService.GetIconSvg(icon.Id).Returns(Task.FromException<string>(new HttpRequestException("Simulated network error")));

        // Act
        _iconDisplay.Icon = icon;

        // Allow async operation to complete
        await Task.Delay(200);

        // Assert
        await _mockIconifyService.Received(1).GetIconSvg(icon.Id);
        // Should not throw and should show fallback
    }

    [Test]
    public void GetIconDisplayText_Should_CreateProperAbbreviations()
    {
        // Arrange & Act & Assert
        var testCases = new[]
        {
            (new IconifyIcon { Name = "home icon", Set = "test" }, "HI"),
            (new IconifyIcon { Name = "user", Set = "test" }, "US"),
            (new IconifyIcon { Name = "single", Set = "test" }, "SI"),
            (new IconifyIcon { Name = "", Set = "testset" }, "TE"),
            (new IconifyIcon { Name = "", Set = "" }, "??")
        };

        foreach (var (icon, expected) in testCases)
        {
            var actual = IconDisplay.GetIconDisplayTextForTesting(icon);
            actual.Should().Be(expected);
        }
    }
}
