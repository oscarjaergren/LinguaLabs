using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Text;

namespace LinguaLabs.Features.Icons;

public partial record IconDisplayModel
{
    private readonly IIconifyService? _iconService;

    public IconDisplayModel(IconifyIcon icon, IIconifyService? iconService = null)
    {
        Icon = icon;
        _iconService = iconService;
    }

    public IconifyIcon Icon { get; }

    public IIconifyService? IconService => _iconService;

    public IFeed<string> DisplayText => Feed.Async<string>(ct => ValueTask.FromResult(GetIconDisplayText(Icon)));

    public IFeed<IconDisplayState> IconState => Feed.Async(GetIconStateAsync); private async ValueTask<IconDisplayState> GetIconStateAsync(CancellationToken ct)
    {
        System.Diagnostics.Debug.WriteLine($"GetIconStateAsync called for icon: {Icon.Id ?? "null"}, has service: {_iconService != null}");

        if (string.IsNullOrEmpty(Icon.Id) || _iconService == null)
        {
            System.Diagnostics.Debug.WriteLine($"Returning fallback state - Empty ID: {string.IsNullOrEmpty(Icon.Id)}, No service: {_iconService == null}");
            return new IconDisplayState(
                IsLoading: false,
                ImageSource: null,
                FallbackText: GetIconDisplayText(Icon),
                ShowImage: false,
                ShowFallback: true,
                HasError: _iconService == null
            );
        }

        try
        {
            System.Diagnostics.Debug.WriteLine($"Loading icon: {Icon.Id}");

            // Download SVG
            var svgContent = await _iconService.GetIconSvg(Icon.Id);

            if (!string.IsNullOrEmpty(svgContent))
            {
                System.Diagnostics.Debug.WriteLine($"SVG content loaded for {Icon.Id}, length: {svgContent.Length}");

                // Try to render SVG
                var imageSource = await TryCreateImageSource(svgContent);
                if (imageSource != null)
                {
                    System.Diagnostics.Debug.WriteLine($"SVG image created successfully for {Icon.Id}");
                    return new IconDisplayState(
                        IsLoading: false,
                        ImageSource: imageSource,
                        FallbackText: GetIconDisplayText(Icon),
                        ShowImage: true,
                        ShowFallback: false,
                        HasError: false
                    );
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"SVG rendering failed for {Icon.Id}, showing indicator");
                    // SVG downloaded but couldn't render - show green indicator
                    return new IconDisplayState(
                        IsLoading: false,
                        ImageSource: null,
                        FallbackText: "📄", // Document icon to indicate SVG found
                        ShowImage: false,
                        ShowFallback: true,
                        HasError: false
                    );
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No SVG content received for {Icon.Id}");
                return new IconDisplayState(
                    IsLoading: false,
                    ImageSource: null,
                    FallbackText: GetIconDisplayText(Icon),
                    ShowImage: false,
                    ShowFallback: true,
                    HasError: true
                );
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading icon {Icon.Id}: {ex.Message}");
            return new IconDisplayState(
                IsLoading: false,
                ImageSource: null,
                FallbackText: GetIconDisplayText(Icon),
                ShowImage: false,
                ShowFallback: true,
                HasError: true
            );
        }
    }
    private ValueTask<BitmapImage?> TryCreateImageSource(string svgContent)
    {
        try
        {
            var cleanSvg = CleanSvgContent(svgContent);
            var svgBytes = Encoding.UTF8.GetBytes(cleanSvg);
            var base64Svg = Convert.ToBase64String(svgBytes);
            var dataUrl = $"data:image/svg+xml;base64,{base64Svg}";

            var bitmap = new BitmapImage();
            bitmap.UriSource = new Uri(dataUrl);

            System.Diagnostics.Debug.WriteLine($"Created BitmapImage with data URL length: {dataUrl.Length}");
            return ValueTask.FromResult<BitmapImage?>(bitmap);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to create image source: {ex.Message}");
            return ValueTask.FromResult<BitmapImage?>(null);
        }
    }

    private string CleanSvgContent(string svgContent)
    {
        if (string.IsNullOrEmpty(svgContent))
            return svgContent;

        // Ensure SVG has proper encoding declaration
        if (!svgContent.Contains("<?xml"))
        {
            svgContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + svgContent;
        }

        // Remove problematic xlink namespace
        svgContent = svgContent.Replace("xmlns:xlink=\"http://www.w3.org/1999/xlink\"", "");

        // Ensure SVG has a viewBox for proper scaling
        if (!svgContent.Contains("viewBox") && svgContent.Contains("<svg"))
        {
            var widthMatch = System.Text.RegularExpressions.Regex.Match(svgContent, @"width=""(\d+)""");
            var heightMatch = System.Text.RegularExpressions.Regex.Match(svgContent, @"height=""(\d+)""");

            if (widthMatch.Success && heightMatch.Success)
            {
                var width = widthMatch.Groups[1].Value;
                var height = heightMatch.Groups[1].Value;
                svgContent = svgContent.Replace("<svg", $"<svg viewBox=\"0 0 {width} {height}\"");
            }
        }

        return svgContent;
    }

    private static string GetIconDisplayText(IconifyIcon icon)
    {
        // Try to create a meaningful visual representation
        if (!string.IsNullOrEmpty(icon.Emoji))
            return icon.Emoji;

        // Use first few characters of the name
        var name = icon.Name.Replace("-", "").Replace("_", "");
        if (name.Length >= 2)
            return name[..Math.Min(2, name.Length)].ToUpper();

        // Fallback to icon set abbreviation
        return icon.Set.Length >= 2 ? icon.Set.Substring(0, 2).ToUpper() : "??";
    }
}

public record IconDisplayState(
    bool IsLoading,
    BitmapImage? ImageSource,
    string FallbackText,
    bool ShowImage,
    bool ShowFallback,
    bool HasError
);
