namespace LinguaLabs.Services.Caching;

public interface IWeatherCache
{
    ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
}
