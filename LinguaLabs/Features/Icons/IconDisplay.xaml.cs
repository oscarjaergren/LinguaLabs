using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace LinguaLabs.Features.Icons;

public sealed partial class IconDisplay : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(IconifyIcon),
            typeof(IconDisplay),
            new PropertyMetadata(null, OnIconChanged));

    public static readonly DependencyProperty IconServiceProperty =
        DependencyProperty.Register(
            nameof(IconService),
            typeof(IIconifyService),
            typeof(IconDisplay),
            new PropertyMetadata(null, OnIconServiceChanged));

    public IconifyIcon Icon
    {
        get => (IconifyIcon)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IIconifyService IconService
    {
        get => (IIconifyService)GetValue(IconServiceProperty);
        set => SetValue(IconServiceProperty, value);
    }    public IconDisplay()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Try to find the IconService in the visual tree when the control is loaded
        if (IconService == null)
        {
            TryFindIconService();
        }
    }

    private void TryFindIconService()
    {
        // Walk up the visual tree to find a parent with IconsModel as DataContext
        var parent = this.Parent;
        while (parent != null)
        {
            if (parent is Microsoft.UI.Xaml.FrameworkElement element && element.DataContext is IconsModel model)
            {
                IconService = model.IconService;
                System.Diagnostics.Debug.WriteLine($"Found IconService from parent DataContext: {IconService != null}");
                break;
            }
            parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(parent);
        }
    }private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"OnIconChanged called: {e.NewValue}");
        if (d is IconDisplay control && e.NewValue is IconifyIcon icon)
        {
            System.Diagnostics.Debug.WriteLine($"Starting LoadIconAsync for: {icon.Id}");
            _ = control.LoadIconAsync(icon);
        }
    }    private static void OnIconServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"OnIconServiceChanged called: {e.NewValue != null}");
        if (d is IconDisplay control)
        {
            // If we have both icon and service, try to load again
            var icon = control.Icon;
            if (icon != null && !string.IsNullOrEmpty(icon.Id))
            {
                System.Diagnostics.Debug.WriteLine($"IconService changed, reloading icon: {icon.Id}");
                _ = control.LoadIconAsync(icon);
            }
        }
    }    private async Task LoadIconAsync(IconifyIcon icon)
    {
        // Debug output
        System.Diagnostics.Debug.WriteLine($"LoadIconAsync called for icon: {icon.Id}, Service: {IconService != null}");
        
        if (string.IsNullOrEmpty(icon.Id) || IconService == null)
        {
            System.Diagnostics.Debug.WriteLine($"Showing fallback - IconId empty: {string.IsNullOrEmpty(icon.Id)}, Service null: {IconService == null}");
            ShowFallback(icon);
            return;
        }

        try
        {
            // Show loading state
            LoadingRing.Visibility = Visibility.Visible;
            IconImage.Visibility = Visibility.Collapsed;
            FallbackText.Visibility = Visibility.Collapsed;

            // Download SVG
            System.Diagnostics.Debug.WriteLine($"Downloading SVG for: {icon.Id}");
            var svgContent = await IconService.GetIconSvg(icon.Id);
            
            System.Diagnostics.Debug.WriteLine($"SVG Content received: {!string.IsNullOrEmpty(svgContent)}, Length: {svgContent?.Length ?? 0}");
            
            if (!string.IsNullOrEmpty(svgContent))
            {
                // For now, let's show a special text indicating that we got the SVG
                // This will help us debug if the service is working
                LoadingRing.Visibility = Visibility.Collapsed;
                IconImage.Visibility = Visibility.Collapsed;
                FallbackText.Visibility = Visibility.Visible;
                FallbackText.Text = "SVG"; // Show this instead of fallback to indicate SVG was downloaded
                FallbackText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                
                System.Diagnostics.Debug.WriteLine("Showing SVG indicator text");
                
                // TODO: Uncomment this when we fix SVG rendering
                /*
                try
                {
                    System.Diagnostics.Debug.WriteLine("Attempting to create SvgImageSource");
                    var svgImageSource = new SvgImageSource();
                    using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgContent));
                    await svgImageSource.SetSourceAsync(stream.AsRandomAccessStream());
                    
                    IconImage.Source = svgImageSource;
                    
                    // Show SVG image
                    LoadingRing.Visibility = Visibility.Collapsed;
                    IconImage.Visibility = Visibility.Visible;
                    FallbackText.Visibility = Visibility.Collapsed;
                    
                    System.Diagnostics.Debug.WriteLine("SVG image set successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SVG loading failed: {ex.Message}");
                    // SVG loading failed, show fallback
                    ShowFallback(icon);
                }
                */
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No SVG content received");
                // No SVG content, show fallback
                ShowFallback(icon);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in LoadIconAsync: {ex.Message}");
            // Error loading, show fallback
            ShowFallback(icon);
        }
    }    private void ShowFallback(IconifyIcon icon)
    {
        LoadingRing.Visibility = Visibility.Collapsed;
        IconImage.Visibility = Visibility.Collapsed;
        FallbackText.Visibility = Visibility.Visible;
        
        // Create a better abbreviation and add debug info
        var displayText = GetIconDisplayText(icon);
        
        // Add debug info to see what's happening
        var debugInfo = $"{displayText}";
        if (IconService == null)
        {
            debugInfo += "⚠"; // Warning symbol if no service
        }
        
        FallbackText.Text = debugInfo;
        FallbackText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
            IconService == null ? Microsoft.UI.Colors.Red : Microsoft.UI.Colors.White);
    }    private static string GetIconDisplayText(IconifyIcon icon)
    {
        if (!string.IsNullOrEmpty(icon.Emoji))
            return icon.Emoji;

        // Use the first two letters of each word in the name
        var words = icon.Name.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length >= 2)
        {
            return (words[0].Substring(0, Math.Min(1, words[0].Length)) + 
                   words[1].Substring(0, Math.Min(1, words[1].Length))).ToUpper();
        }
        else if (words.Length == 1 && words[0].Length >= 2)
        {
            return words[0].Substring(0, 2).ToUpper();
        }
        
        return icon.Set?.Substring(0, Math.Min(2, icon.Set.Length)).ToUpper() ?? "??";
    }

    // Expose method for testing
    public static string GetIconDisplayTextForTesting(IconifyIcon icon) => GetIconDisplayText(icon);
}