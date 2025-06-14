using Microsoft.UI.Xaml.Data;

namespace LinguaLabs.Converters;
class BoolToResourceConverter : IValueConverter
{
	public string? TrueValue { get; set; } = null;
	public string? FalseValue { get; set; } = null;
	public string? NullValue { get; set; } = null;

	public object? Convert(object value, Type targetType, object parameter, string language)
	{
		return value is bool bValue
			? ResolveResource(bValue ? TrueValue : FalseValue)
			: ResolveResource(NullValue);
	}

	private object? ResolveResource(string? name)
		=> Application.Current.Resources.TryGetValue(name, out object? value) ? value : null;

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotImplementedException("Only one-way conversion is supported.");
}
