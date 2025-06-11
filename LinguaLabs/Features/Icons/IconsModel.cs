namespace LinguaLabs.Features.Icons;

public partial record IconsModel
{
    private readonly INavigator _navigator;
    private readonly IIconifyService _iconService;

    public IconsModel(INavigator navigator, IIconifyService iconService)
    {
        _navigator = navigator;
        _iconService = iconService;
    }

    public IState<string> Term => State<string>.Empty(this);

    public IState<IconifyIcon?> SelectedIcon => State<IconifyIcon?>.Empty(this);

    public IState<string> SelectedCategory => State<string>.Value(this, () => "all");

    public IState<bool> IsLoading => State<bool>.Empty(this);    public IListFeed<IconifyIcon> Results => Term
        .Where(term => term is { Length: > 0 })
        .SelectAsync(Search)
        .AsListFeed();

    public IFeed<bool> HasResults => Results
        .Select(icons => icons.Any());

    public IFeed<bool> ShowEmptyState => 
        Feed.Combine(Term, IsLoading)
            .Select((term, loading) => term.Length > 0 && !loading)
            .CombineLatest(HasResults)
            .Select((showCheck, hasResults) => showCheck && !hasResults);

    public IFeed<string> ResultsCountText => Results
        .Select(icons => $"{icons.Count} results");

    public IFeed<bool> CanSelect => SelectedIcon.Select(icon => icon != null);

    public IFeed<string> SelectedIconName => SelectedIcon
        .Select(icon => icon?.Name ?? "No icon selected");

    public IFeed<string> SelectedIconInfo => SelectedIcon
        .Select(icon => icon != null ? $"{icon.Set} • {icon.Category}" : "Select an icon above");

    public IFeed<string> SelectedIconDisplay => SelectedIcon
        .Select(icon => icon != null ? GetIconDisplayText(icon) : "");    public async ValueTask SelectIcon(IconifyIcon icon)
    {
        await SelectedIcon.SetAsync(icon, CancellationToken.None);
    }

    public async ValueTask ClearSelection()
    {
        await SelectedIcon.SetAsync(null, CancellationToken.None);
    }

    public async ValueTask SetCategory(string category)
    {
        await SelectedCategory.SetAsync(category);
        
        // Update search term based on category
        var searchTerm = category == "all" ? "" : category;
        await Term.SetAsync(searchTerm);
    }

    public async ValueTask SetSearchTerm(string term)
    {
        await IsLoading.SetAsync(!string.IsNullOrEmpty(term));
        await Term.SetAsync(term);
    }

    // Public search method for direct use
    public async ValueTask<IImmutableList<IconifyIcon>> SearchIcons(string term, CancellationToken ct)
    {
        return await Search(term, ct);
    }

    private async ValueTask<IImmutableList<IconifyIcon>> Search(string term, CancellationToken ct)
    {
        try
        {
            await IsLoading.SetAsync(true);
            var results = await _iconService.Search(term, ct);
            return results.ToImmutableList();
        }
        finally
        {
            await IsLoading.SetAsync(false);
        }
    }

    private static string GetIconDisplayText(IconifyIcon icon)
    {
        if (!string.IsNullOrEmpty(icon.Emoji))
            return icon.Emoji;
        
        // Show abbreviated icon name
        return icon.Name.Length > 4 ? icon.Name.Substring(0, 4) : icon.Name;
    }
}
