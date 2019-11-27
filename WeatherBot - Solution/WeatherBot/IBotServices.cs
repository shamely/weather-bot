using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using WeatherBot.Services.WeatherStack;

namespace WeatherBot
{
	public interface IBotServices
	{
		LuisRecognizer LuisService { get; }
		QnAMaker QnAMakerService { get; }
		WeatherStackServices WeatherStackServices { get; }
	}
}