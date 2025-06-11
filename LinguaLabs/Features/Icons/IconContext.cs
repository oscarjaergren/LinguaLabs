using System.Text.Json.Serialization;

namespace LinguaLabs.Features.Icons;

/// <summary>
/// Generated class for System.Text.Json Serialization
/// </summary>
/// <remarks>
/// When using the JsonSerializerContext you must add the JsonSerializableAttribute
/// for each type that you may need to serialize / deserialize including both the
/// concrete type and any interface that the concrete type implements.
/// For more information on the JsonSerializerContext see:
/// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation?WT.mc_id=DT-MVP-5002924
/// </remarks>
[JsonSerializable(typeof(IconifyIcon))]
[JsonSerializable(typeof(IconifyIcon[]))]
[JsonSerializable(typeof(IEnumerable<IconifyIcon>))]
[JsonSerializable(typeof(IImmutableList<IconifyIcon>))]
[JsonSerializable(typeof(ImmutableList<IconifyIcon>))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class IconContext : JsonSerializerContext;
