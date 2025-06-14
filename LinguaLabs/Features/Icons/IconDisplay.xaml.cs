using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace LinguaLabs.Features.Icons;

public sealed partial class IconDisplay : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(IconifyIcon),
            typeof(IconDisplay),
            new PropertyMetadata(null, OnIconChanged)); public static readonly DependencyProperty IconServiceProperty =
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

    public IIconifyService? IconService
    {
        get => (IIconifyService?)GetValue(IconServiceProperty);
        set => SetValue(IconServiceProperty, value);
    }

    private IconDisplayModel? _model; public IconDisplay()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Get the service from the dependency injection container if not already set
        if (IconService == null)
        {
            try
            {
                var service = ((App)Application.Current).Host?.Services?.GetService<IIconifyService>();
                if (service != null)
                {
                    IconService = service;
                    System.Diagnostics.Debug.WriteLine("IconService resolved from DI container");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to resolve IconService from DI container");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resolving IconService: {ex.Message}");
            }
        }

        // Update model with current icon and service
        UpdateModel();
    }

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"OnIconChanged called: {e.NewValue}");
        if (d is IconDisplay control)
        {
            try
            {
                if (e.NewValue is IconifyIcon icon)
                {
                    var iconId = icon.Id ?? "null";
                    System.Diagnostics.Debug.WriteLine($"Updating model for icon: {iconId}");
                }
                control.UpdateModel();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnIconChanged: {ex.Message}");
            }
        }
    }

    private static void OnIconServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IconDisplay control)
        {
            System.Diagnostics.Debug.WriteLine($"IconService changed: {e.NewValue != null}");
            control.UpdateModel();
        }
    }
    private void UpdateModel()
    {
        if (Icon != null)
        {
            // Only create a new model if we don't have one or if the icon or service changed
            if (_model == null || _model.Icon != Icon || (_model.IconService != IconService))
            {
                _model = new IconDisplayModel(Icon, IconService);
                this.DataContext = _model;

                var iconId = Icon.Id ?? "null";
                System.Diagnostics.Debug.WriteLine($"Model updated for icon: {iconId}, has service: {IconService != null}");
            }
        }
        else
        {
            if (_model != null)
            {
                _model = null;
                this.DataContext = null;
                System.Diagnostics.Debug.WriteLine("Model cleared - no icon");
            }
        }
    }
}
