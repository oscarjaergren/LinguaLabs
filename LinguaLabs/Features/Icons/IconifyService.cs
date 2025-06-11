
namespace LinguaLabs.Features.Icons;

public sealed class IconifyService(
    ISerializer serializer,
    ILogger<WeatherCache> logger
    ) : IIconifyService
{
    private readonly ISerializer _serializer = serializer;
    private readonly ILogger _logger = logger;

    private static readonly HttpClient httpClient = new();

    public async Task<string> GetIconSvg(string iconId)
    {
        try
        {
            var url = $"https://api.iconify.design/{iconId}.svg";
            return await httpClient.GetStringAsync(url);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public async ValueTask<IImmutableList<IconifyIcon>> Search(string searchTerm, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return [];

        var searchUrl = $"https://api.iconify.design/search?query={Uri.EscapeDataString(searchTerm)}";
        var response = await httpClient.GetStringAsync(searchUrl);
        return _serializer.FromString<ImmutableArray<IconifyIcon>>(response);
    }
}
