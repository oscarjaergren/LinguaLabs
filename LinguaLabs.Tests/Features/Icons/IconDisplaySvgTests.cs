using LinguaLabs.Features.Icons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NSubstitute;

namespace LinguaLabs.Tests.Features.Icons;

public class IconDisplaySvgTests
{
    private IIconifyService _mockIconifyService = null!;
    private IconDisplay _iconDisplay = null!;

    [Before(Test)]
    public void Setup()
    {
        _mockIconifyService = Substitute.For<IIconifyService>();
        _iconDisplay = new IconDisplay();
    }

    [Test]
    public async Task CleanSvgContent_Should_Add_XmlDeclaration_When_Missing()
    {
        // Arrange
        var svgWithoutDeclaration = "<svg width='24' height='24'><rect width='24' height='24'/></svg>";
        
        // Act - Using reflection to access the private method for testing
        var method = typeof(IconDisplay).GetMethod("CleanSvgContent", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = method?.Invoke(_iconDisplay, new object[] { svgWithoutDeclaration }) as string;
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Contains("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")).IsTrue();
    }

    [Test]
    public async Task CleanSvgContent_Should_Add_ViewBox_When_Missing()
    {
        // Arrange
        var svgWithoutViewBox = "<svg width=\"24\" height=\"24\"><rect width='24' height='24'/></svg>";
        
        // Act - Using reflection to access the private method for testing
        var method = typeof(IconDisplay).GetMethod("CleanSvgContent", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = method?.Invoke(_iconDisplay, new object[] { svgWithoutViewBox }) as string;
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Contains("viewBox=\"0 0 24 24\"")).IsTrue();
    }

    [Test]
    public async Task GetIconDisplayText_Should_Return_Emoji_When_Available()
    {
        // Arrange
        var icon = new IconifyIcon 
        { 
            Id = "test:icon", 
            Name = "Test Icon", 
            Emoji = "🔥",
            Set = "test"
        };

        // Act
        var result = IconDisplay.GetIconDisplayTextForTesting(icon);

        // Assert
        await Assert.That(result).IsEqualTo("🔥");
    }

    [Test]
    public async Task GetIconDisplayText_Should_Return_Abbreviation_When_No_Emoji()
    {
        // Arrange
        var icon = new IconifyIcon 
        { 
            Id = "test:icon", 
            Name = "Test Icon", 
            Emoji = "",
            Set = "test"
        };

        // Act
        var result = IconDisplay.GetIconDisplayTextForTesting(icon);

        // Assert
        await Assert.That(result).IsEqualTo("TI"); // First letters of "Test Icon"
    }

    [Test]
    public async Task GetIconDisplayText_Should_Handle_Single_Word_Names()
    {
        // Arrange
        var icon = new IconifyIcon 
        { 
            Id = "test:icon", 
            Name = "TestIcon", 
            Emoji = "",
            Set = "test"
        };

        // Act
        var result = IconDisplay.GetIconDisplayTextForTesting(icon);

        // Assert
        await Assert.That(result).IsEqualTo("TE"); // First two letters of "TestIcon"
    }

    [Test]
    public async Task GetIconDisplayText_Should_Fallback_To_Set_When_Name_Too_Short()
    {
        // Arrange
        var icon = new IconifyIcon 
        { 
            Id = "test:icon", 
            Name = "T", 
            Emoji = "",
            Set = "testset"
        };

        // Act
        var result = IconDisplay.GetIconDisplayTextForTesting(icon);

        // Assert
        await Assert.That(result).IsEqualTo("TE"); // First two letters of set
    }
}
