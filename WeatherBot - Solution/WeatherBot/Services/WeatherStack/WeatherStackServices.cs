using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherBot.Services.WeatherStack
{
	public class WeatherStackServices
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;

		public WeatherStackServices(string baseUrl, string apiKey)
		{
			_httpClient = new HttpClient(){BaseAddress = new Uri(baseUrl) };
			_apiKey = apiKey;
		}

		public async Task<WeatherDetails> GetForecastWeather(string location)
		{
			var request = $"forecast?access_key={_apiKey}&query={location}";
			var response = await _httpClient.GetAsync(request);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var result = JsonConvert.DeserializeObject<WeatherStackResult>(content);

				return result != null ? WeatherDetailsMapper.MapToDetails(result) : null;
			}

			return null;
		}
	}
}