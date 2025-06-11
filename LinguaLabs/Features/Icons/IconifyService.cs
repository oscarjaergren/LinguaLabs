
namespace LinguaLabs.Features.Icons;

public sealed class IconifyService(
    ISerializer serializer,
    ILogger<IconifyService> logger
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
    }    public async ValueTask<IImmutableList<IconifyIcon>> Search(string searchTerm, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return [];

        try
        {
            var searchUrl = $"https://api.iconify.design/search?query={Uri.EscapeDataString(searchTerm)}";
            var response = await httpClient.GetStringAsync(searchUrl, ct);
            
            var searchResponse = _serializer.FromString<IconifySearchResponse>(response);
            
            // Convert the response to IconifyIcon objects
            var icons = new List<IconifyIcon>();
            
            foreach (var iconId in searchResponse.Icons)
            {
                // Parse icon ID (format: "collection:icon-name")
                var parts = iconId.Split(':', 2);
                if (parts.Length != 2) continue;
                
                var collectionKey = parts[0];
                var iconName = parts[1];
                
                // Get collection info if available
                var collection = searchResponse.Collections.GetValueOrDefault(collectionKey);
                
                icons.Add(new IconifyIcon
                {
                    Id = iconId,
                    Name = iconName.Replace('-', ' ').Replace('_', ' '), // Make name more readable
                    Set = collectionKey,
                    Category = collection?.Category ?? "Unknown",
                    Tags = collection?.Tags != null ? string.Join(", ", collection.Tags) : "",
                    Emoji = "" // Iconify icons don't have emoji equivalents by default
                });
            }
            
            return icons.ToImmutableList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search icons for term: {SearchTerm}", searchTerm);
            return [];
        }
    }
}
