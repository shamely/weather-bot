using System.Linq;

namespace WeatherBot.CognitiveModels
{
	public partial class Weather
	{
		public string PlaceEntities => Entities?._instance?.Places?.FirstOrDefault()?.Text;
	}
}