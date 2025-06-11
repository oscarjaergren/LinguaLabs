namespace LinguaLabs.Features.Icons;

public partial record IconsModel
{
    private readonly INavigator _navigator;
    private readonly IIconifyService _iconService;

    public IIconifyService IconService => _iconService;

    public IconsModel(INavigator navigator, IIconifyService iconService)
    {
        _navigator = navigator;
        _iconService = iconService;
    }

    public IState<string> Term => State<string>.Empty(this);

    public IState<IconifyIcon> SelectedIcon => State<IconifyIcon>.Value(this, () => new IconifyIcon { Id = "", Name = "", Set = "", Category = "" });

    public IState<string> SelectedCategory => State<string>.Value(this, () => "all");

    public IState<bool> IsLoading => State<bool>.Empty(this);    public IListFeed<IconifyIcon> Results => Term
            .Where(term => term is { Length: > 0 })
            .SelectAsync(Search)
            .AsListFeed();

    public IFeed<bool> HasResults => Results
        .AsFeed()
        .Select(icons => icons?.Any() ?? false);

    public IFeed<bool> ShowEmptyState =>
        Feed.Combine(Term, IsLoading, HasResults)
            .Select(values => values.Item1.Length > 0 && !values.Item2 && !values.Item3);

    public IFeed<string> ResultsCountText => Results
        .AsFeed()
        .Select(icons => $"{icons?.Count ?? 0} results");

    public IFeed<bool> CanSelect => SelectedIcon.Select(icon => !string.IsNullOrEmpty(icon.Id));

    public IFeed<string> SelectedIconName => SelectedIcon
        .Select(icon => !string.IsNullOrEmpty(icon.Name) ? icon.Name : "No icon selected");

    public IFeed<string> SelectedIconInfo => SelectedIcon
        .Select(icon => !string.IsNullOrEmpty(icon.Id) ? $"{icon.Set} • {icon.Category}" : "Select an icon above");

    public IFeed<string> SelectedIconDisplay => SelectedIcon
        .Select(icon => !string.IsNullOrEmpty(icon.Id) ? GetIconDisplayText(icon) : "");

    public async ValueTask SelectIcon(IconifyIcon icon)
    {
        await SelectedIcon.Update(_ => icon, CancellationToken.None);
    }

    public async ValueTask ClearSelection()
    {
        await SelectedIcon.Update(_ => new IconifyIcon { Id = "", Name = "", Set = "", Category = "" }, CancellationToken.None);
    }

    public async ValueTask SetCategory(string category)
    {
        await SelectedCategory.Update(_ => category, CancellationToken.None);

        // Update search term based on category
        var searchTerm = category == "all" ? "" : category;
        await Term.Update(_ => searchTerm, CancellationToken.None);
    }

    public async ValueTask SetSearchTerm(string term)
    {
        await IsLoading.SetAsync(!string.IsNullOrEmpty(term), CancellationToken.None);
        await Term.Update(_ => term, CancellationToken.None);
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
            await IsLoading.SetAsync(true, ct);
            var results = await _iconService.Search(term, ct);
            return results.ToImmutableList();
        }
        finally
        {
            await IsLoading.SetAsync(false, CancellationToken.None);
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
