namespace LinguaLabs.Features.Icons;

public partial record IconsModel
{
    private readonly INavigator _navigator;
    private readonly IIconifyService _iconService;
    //private readonly IMessenger _messenger;

    public IconsModel(INavigator navigator, IIconifyService iconService)
    {
        _navigator = navigator;
        _iconService = iconService;
        //_messenger = messenger;

        //Filter = State.Value(this, () => filter ?? new SearchFilter())
        //    .Observe(_messenger, f => f);
    }

    public IState<string> Term => State<string>.Empty(this);

    public IListFeed<IconifyIcon> Results => Term
        .Where(term => term is { Length: > 0 })
        .SelectAsync(Search)
        .AsListFeed();

    //public IFeed<bool> Searched => Feed.Combine(Filter, Term).Select(GetSearched);

    //public IFeed<bool> HasFilter => Filter.Select(f => f.HasFilter);

    //public IListFeed<string> SearchHistory => ListFeed.Async(async ct => _iconService.GetSearchHistory());

    //public async ValueTask ApplyHistory(string term) => await Term.SetAsync(term);

    private async ValueTask<IImmutableList<IconifyIcon>> Search(string term, CancellationToken ct)
    {
        var searchedRecipes = await _iconService.Search(term, ct);
        return searchedRecipes.ToImmutableList();
    }

    //private bool GetSearched((SearchFilter filter, string term) inputs) => inputs.filter.HasFilter || !inputs.term.IsNullOrEmpty();

    //public async ValueTask ResetFilters() =>
    //    await Filter.UpdateAsync(current => new SearchFilter());
}
