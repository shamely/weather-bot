using System.Linq;

namespace WeatherBot.CognitiveModels
{
	public partial class WeatherLuis
	{
		public string PlaceEntities => Entities?._instance?.Place.FirstOrDefault()?.Text;
		public string GeographyEntities => Entities?.geography?.Location;
	}
}