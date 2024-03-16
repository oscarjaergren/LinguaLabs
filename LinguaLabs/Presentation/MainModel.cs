using System.Diagnostics;

namespace LinguaLabs.Presentation;

public partial record MainModel
{
    private INavigator _navigator;

    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }

    public IState<string> Name => State<string>.Value(this, () => string.Empty);

    public async Task GoToSecond()
    {
        var name = await Name;
        await _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(name!));
    }

    // Handle swipe left action
    // This method will be called when the user swipes the card to the left
    public void OnSwipeLeft(object sender, EventArgs args)
    {
        Debug.WriteLine("Swiped left");
    }

    // Handle swipe right action
    // This method will be called when the user swipes the card to the right
    public void OnSwipeRight(object sender, EventArgs args)
    {
        Debug.WriteLine("Swiped Right");
    }

}
