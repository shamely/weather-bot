using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WeatherBot.WeatherStack
{
	public class WeatherStackApiServices
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;

		public WeatherStackApiServices(IConfiguration configuration)
		{
			_httpClient = new HttpClient() {BaseAddress = new Uri(configuration["weatherStackHost"])};
			_apiKey = configuration["weatherStackApiKey"];
		}

		public async Task<WeatherDetails> GetForecastWeather(string location)
		{
			var request = $"forecast?access_key={_apiKey}&query={location}";
			var response = await _httpClient.GetAsync(request);

			if (!response.IsSuccessStatusCode) return null;

			var content = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<WeatherStackApiResults>(content);

			return result != null ? WeatherDetails.MapWeatherDetails(result) : null;
		}
	}
}