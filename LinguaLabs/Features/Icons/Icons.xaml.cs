using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.ComponentModel;

namespace LinguaLabs.Features.Icons;

public sealed partial class Icons : Flyout
{
    public Icons()
    {
        InitializeComponent();
    }
    
    private void IconButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is IconifyIcon icon)
        {
            // Later this can be enhanced to pass the selected icon back to the caller
            this.Hide();
            // You could raise an event here or use a callback mechanism
            // to notify the parent that an icon was selected
        }
    }
}
