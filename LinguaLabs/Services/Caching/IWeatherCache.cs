namespace LinguaLabs.Services.Caching;
using WeatherForecast = LinguaLabs.Client.Models.WeatherForecast;
public interface IWeatherCache
{
    ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
}
