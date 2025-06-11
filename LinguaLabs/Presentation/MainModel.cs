using System.Diagnostics;
using LinguaLabs.Features.Icons;

namespace LinguaLabs.Presentation;

public partial record MainModel
{
    private readonly INavigator _navigator;

    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IAuthenticationService authentication,
        INavigator navigator)
    {
        _navigator = navigator;
        _authentication = authentication;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }

    public IState<string> Name => State<string>.Value(this, () => string.Empty);    public IState<int> Count => State<int>.Value(this, () => 0);    public IState<string> SelectedIconId => State<string>.Value(this, () => string.Empty);

    public IFeed<string> CounterText => Count.Select(_currentCount => _currentCount switch
    {
        0 => "Press Me",
        1 => "Pressed Once!",
        _ => $"Pressed {_currentCount} times!"
    });

    public IFeed<string> SelectedIconText => SelectedIconId.Select(iconId => string.IsNullOrEmpty(iconId) 
        ? "No icon selected" 
        : $"Selected: {iconId}");

    public async Task Counter(CancellationToken ct) =>
        await Count.Update(x => ++x, ct);    public async Task SetIconCommand()
    {
        // Get selected icon from the Icons flyout
        var selectedIcon = await _navigator.GetDataAsync<IconsModel, IconifyIcon?>(this, qualifier: Qualifiers.Dialog);
        if (selectedIcon != null)
        {
            await SelectedIconId.Update(x => selectedIcon.Id, CancellationToken.None);
        }
    }

    //public IState<Card> Entity { get; }

    public async ValueTask CreateTask(CancellationToken ct)
    {
        //var list = await Entity;
        //if (list is null)
        //{
        //    return;
        //}

        var taskName = await _navigator.GetDataAsync<IconsModel, string>(this, qualifier: Qualifiers.Dialog, cancellation: ct);
        //if (taskName is { Length: > 0 })
        //{
        //    var newTask = new ToDoTask { Title = taskName };
        //    await _taskSvc.CreateAsync(list, newTask, ct);
        //}
    }

    public async Task GoToSecond()
    {
        var name = await Name;
        await _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(name!));
    }

    public async ValueTask Logout(CancellationToken token)
    {
        await _authentication.LogoutAsync(token);
    }

    private readonly IAuthenticationService _authentication;
}
