using System;
using System.Linq;

namespace WeatherBot.WeatherStack
{
	public class WeatherDetails
	{
		public string PlaceName { get; set; }
		public DateTime ForecastDate { get; set; }
		public string WeatherIconUrl { get; set; }
		public int CurrentTemp { get; set; }
		public int MaxTemp { get; set; }
		public int MinTemp { get; set; }

		public static WeatherDetails MapWeatherDetails(WeatherStackApiResults result)
		{
			var nearestForecast = result.forecast?.FirstOrDefault().Value;
			if (nearestForecast == null) return null;

			DateTime.TryParse(nearestForecast.date ?? "", out var dateResult);

			return new WeatherDetails
			{
				PlaceName = result.location?.name,
				ForecastDate = dateResult,
				WeatherIconUrl = result.current?.weather_icons?.FirstOrDefault(),
				CurrentTemp = result.current?.temperature ?? 0,
				MaxTemp = nearestForecast.maxtemp,
				MinTemp = nearestForecast.mintemp
			};
		}
	}
}