namespace LinguaLabs.Features.Icons;

public interface IIconifyService
{
    ValueTask<IImmutableList<IconifyIcon>> Search(string searchTerm, CancellationToken ct);

    Task<string> GetIconSvg(string iconId);
}
