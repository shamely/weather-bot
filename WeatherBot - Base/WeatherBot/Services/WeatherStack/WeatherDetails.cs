using System;
using System.Linq;

namespace WeatherBot.Services.WeatherStack
{
	public class WeatherDetails
	{
		public string PlaceName { get; set; }
		public DateTime ForecastDate { get; set; }
		public string WeatherIconUrl { get; set; }
		public int CurrentTemp { get; set; }
		public int MaxTemp { get; set; }
		public int MinTemp { get; set; }
	}

	public static class WeatherDetailsMapper
	{
		public static WeatherDetails MapToDetails(WeatherStackResult result)
		{
			var nearestForecast = result.forecast?.FirstOrDefault().Value;
			if (nearestForecast == null) return null;

			DateTime.TryParse(nearestForecast?.date ?? "", out var dateResult);

			return new WeatherDetails()
			{
				PlaceName = result.location?.name,
				ForecastDate = dateResult,
				WeatherIconUrl = result.current?.weather_icons?.FirstOrDefault(),
				CurrentTemp = result.current?.temperature ?? 0,
				MaxTemp = nearestForecast?.maxtemp ?? 0,
				MinTemp = nearestForecast?.mintemp ?? 0
			};
		}
	}
}