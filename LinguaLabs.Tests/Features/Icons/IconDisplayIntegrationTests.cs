using LinguaLabs.Features.Icons;
using Microsoft.UI.Xaml.Controls;
using Uno.Extensions.Navigation;

namespace LinguaLabs.Tests.Features.Icons;

public class IconDisplayIntegrationTests
{
    private IIconifyService _mockIconifyService = null!;
    private INavigator _mockNavigator = null!;
    private IconsModel _iconsModel = null!;

    [Before(Test)]
    public void Setup()
    {
        _mockIconifyService = Substitute.For<IIconifyService>();
        _mockNavigator = Substitute.For<INavigator>();
        _iconsModel = new IconsModel(_mockNavigator, _mockIconifyService);
    }

    [Test]
    public async Task IconDisplay_Should_GetService_FromIconsModel()
    {
        // Arrange
        var icon = new IconifyIcon
        {
            Id = "test:icon",
            Name = "test icon",
            Set = "test"
        };

        // Create IconDisplay and simulate it being in a visual tree with IconsModel
        var iconDisplay = new IconDisplay();
        
        // Simulate the DataContext binding that would happen in real UI
        iconDisplay.IconService = _iconsModel.IconService;

        var expectedSvg = "<svg><rect width='100' height='100'/></svg>";
        _mockIconifyService.GetIconSvg(icon.Id).Returns(expectedSvg);

        // Act
        iconDisplay.Icon = icon;
        
        // Allow async operation to complete
        await Task.Delay(200);

        // Assert
        await _mockIconifyService.Received(1).GetIconSvg(icon.Id);
        await Assert.That(iconDisplay.IconService).IsNotNull();
        iconDisplay.IconService.Should().BeSameAs(_mockIconifyService);
    }

    [Test]
    public async Task IconDisplay_Should_FailGracefully_When_ServiceNotAvailable()
    {
        // Arrange - This simulates the exact bug condition from your logs
        var icon = new IconifyIcon
        {
            Id = "tabler:chalkboard-teacher",
            Name = "chalkboard teacher",
            Set = "tabler"
        };

        var iconDisplay = new IconDisplay();
        // Do NOT set IconService - this simulates the DI/binding failure

        // Act
        iconDisplay.Icon = icon;
        
        // Allow UI update time
        await Task.Delay(100);

        // Assert - This test captures the bug scenario
        await Assert.That(iconDisplay.IconService).IsNull();
        
        // Verify service was never called (can't call null service)
        await _mockIconifyService.DidNotReceive().GetIconSvg(Arg.Any<string>());
    }
}
