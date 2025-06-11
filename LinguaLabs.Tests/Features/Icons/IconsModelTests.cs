using LinguaLabs.Features.Icons;
using LinguaLabs.Presentation;
using Uno.Extensions.Reactive;
using Uno.Extensions.Navigation;
using System.Collections.Immutable;

namespace LinguaLabs.Tests.Features.Icons;

public class IconsModelTests
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
    public async Task IconsModel_Should_ExposeIconService()
    {
        // Arrange & Act
        var iconService = _iconsModel.IconService;

        // Assert
        await Assert.That(iconService).IsNotNull();
        iconService.Should().BeSameAs(_mockIconifyService);
    }

    [Test]
    public async Task Term_Should_InitializeEmpty()
    {
        // Arrange & Act
        var termValue = await _iconsModel.Term;

        // Assert
        await Assert.That(termValue).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Results_Should_InitializeEmpty()
    {
        // Arrange & Act
        var resultsValue = await _iconsModel.Results;

        // Assert
        await Assert.That(resultsValue).IsNotNull();
        await Assert.That(resultsValue).HasCount().EqualTo(0);
    }

    [Test]
    public async Task Search_Should_UpdateResults_WhenTermProvided()
    {
        // Arrange
        var searchTerm = "test";
        var expectedIcons = new[]
        {
            new IconifyIcon { Id = "test:icon1", Name = "icon1", Set = "test" },
            new IconifyIcon { Id = "test:icon2", Name = "icon2", Set = "test" }
        }.ToImmutableList();

        _mockIconifyService.Search(searchTerm, Arg.Any<CancellationToken>()).Returns(expectedIcons);

        // Act
        await _iconsModel.Term.Update(_ => searchTerm, CancellationToken.None);

        // Allow reactive chain to complete
        await Task.Delay(100);

        // Assert
        var resultsValue = await _iconsModel.Results;
        await Assert.That(resultsValue).HasCount().EqualTo(2);
        await Assert.That(resultsValue[0].Id).IsEqualTo("test:icon1");
        await _mockIconifyService.Received(1).Search(searchTerm, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Search_Should_NotCallService_WhenTermEmpty()
    {
        // Arrange
        var emptyTerm = "";

        // Act
        await _iconsModel.Term.Update(_ => emptyTerm, CancellationToken.None);

        // Allow reactive chain to complete
        await Task.Delay(100);

        // Assert
        var resultsValue = await _iconsModel.Results;
        await Assert.That(resultsValue).HasCount().EqualTo(0);
        await _mockIconifyService.DidNotReceive().Search(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Search_Should_HandleServiceExceptions()
    {
        // Arrange
        var searchTerm = "error";
        _mockIconifyService.Search(searchTerm, Arg.Any<CancellationToken>())
            .Returns(_ => ValueTask.FromException<IImmutableList<IconifyIcon>>(new HttpRequestException("Network error")));

        // Act
        await _iconsModel.Term.Update(_ => searchTerm, CancellationToken.None);

        // Allow reactive chain to complete
        await Task.Delay(100);

        // Assert - Should not throw and return empty results
        var resultsValue = await _iconsModel.Results;
        await Assert.That(resultsValue).IsNotNull();
        await _mockIconifyService.Received(1).Search(searchTerm, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Results_Should_UpdateReactively_WhenTermChanges()
    {
        // Arrange
        var term1 = "first";
        var term2 = "second";

        var icons1 = new[] { new IconifyIcon { Id = "test:first", Name = "first", Set = "test" } }.ToImmutableList();
        var icons2 = new[] { new IconifyIcon { Id = "test:second", Name = "second", Set = "test" } }.ToImmutableList();

        _mockIconifyService.Search(term1, Arg.Any<CancellationToken>()).Returns(icons1);
        _mockIconifyService.Search(term2, Arg.Any<CancellationToken>()).Returns(icons2);

        // Act & Assert - First search
        await _iconsModel.Term.Update(_ => term1, CancellationToken.None);
        await Task.Delay(100);

        var firstResult = await _iconsModel.Results;
        await Assert.That(firstResult).HasCount().EqualTo(1);
        await Assert.That(firstResult[0].Name).IsEqualTo("first");

        // Act & Assert - Second search
        await _iconsModel.Term.Update(_ => term2, CancellationToken.None);
        await Task.Delay(100);

        var secondResult = await _iconsModel.Results;
        await Assert.That(secondResult).HasCount().EqualTo(1);
        await Assert.That(secondResult[0].Name).IsEqualTo("second");

        await _mockIconifyService.Received(1).Search(term1, Arg.Any<CancellationToken>());
        await _mockIconifyService.Received(1).Search(term2, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Results_Should_ClearResults_WhenTermBecomesEmpty()
    {
        // Arrange
        var searchTerm = "test";
        var expectedIcons = new[]
        {
            new IconifyIcon { Id = "test:icon", Name = "icon", Set = "test" }
        }.ToImmutableList();

        _mockIconifyService.Search(searchTerm, Arg.Any<CancellationToken>()).Returns(expectedIcons);

        // Act - Set term to get results
        await _iconsModel.Term.Update(_ => searchTerm, CancellationToken.None);
        await Task.Delay(100);

        var resultsWithTerm = await _iconsModel.Results;
        await Assert.That(resultsWithTerm).HasCount().EqualTo(1);

        // Act - Clear term
        await _iconsModel.Term.Update(_ => "", CancellationToken.None);
        await Task.Delay(100);

        // Assert
        var resultsAfterClear = await _iconsModel.Results;
        await Assert.That(resultsAfterClear).HasCount().EqualTo(0);
    }

    [Test]
    public async Task SelectedIcon_Should_UpdateCorrectly()
    {
        // Arrange
        var testIcon = new IconifyIcon { Id = "test:icon", Name = "test icon", Set = "test" };

        // Act
        await _iconsModel.SelectIcon(testIcon);

        // Assert
        var selectedIcon = await _iconsModel.SelectedIcon;
        await Assert.That(selectedIcon.Id).IsEqualTo("test:icon");
        await Assert.That(selectedIcon.Name).IsEqualTo("test icon");
    }

    [Test]
    public async Task ClearSelection_Should_ResetSelectedIcon()
    {
        // Arrange
        var testIcon = new IconifyIcon { Id = "test:icon", Name = "test icon", Set = "test" };
        await _iconsModel.SelectIcon(testIcon);

        // Act
        await _iconsModel.ClearSelection();

        // Assert
        var selectedIcon = await _iconsModel.SelectedIcon;
        await Assert.That(selectedIcon.Id).IsEqualTo("");
        await Assert.That(selectedIcon.Name).IsEqualTo("");
    }
}
