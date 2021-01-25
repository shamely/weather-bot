using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using WeatherBot.Cards;
using WeatherBot.CognitiveModels;
using WeatherBot.WeatherStack;

namespace WeatherBot.Dialogs
{
	public class WeatherDialog : ComponentDialog
	{
		private Weather _luisResult;
		private readonly WeatherStackApiServices _weatherStackApiServices;

		private const string GetPlaceMsgText = "Could you tell me from which city you'd like to get the weather? 🌞";
		private const string RecapPlaceMsgText = "I understood you want the weather from: ";
		private const string SomethingWentWrongMsgText = "Something went wrong while fetching the weather forecast... 😞";
		private const string ColdWeatherMsgTxt = "Brrr 🥶, looks like it's cold today. Better wear a hot sweater 🧶";
		private const string HotWeatherMsgTxt = "Mhhh, I love a nice warm weather ☀️😎. Don't you?";

		public WeatherDialog(WeatherStackApiServices weatherStackApiServices, string id) : base(id)
		{
			_weatherStackApiServices = weatherStackApiServices;

			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				GetPlaceStepAsync,
				ShowWeatherAsync
			}));

			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> GetPlaceStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			// TODO Get the Luis typed result from the options
			// Hint -> It's in the context

			// TODO Ask for the place if not recognized as an entity
			

			// Skip the wait and go directly to next step.
			return await stepContext.NextAsync(_luisResult.PlaceEntities, cancellationToken);
		}

		private async Task<DialogTurnResult> ShowWeatherAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			// TODO Grab the Place from the result and display the recap message
			// Hint -> It's in the context

			// TODO Call the Weather Stack Api Method with the place

			// TODO If the result is null, display the something went wrong message and return end of the dialog

			// TODO Else display an attachment with the card
			// Hint -> MessageFactory
			// Hint -> The Attachment method is in WeatherCards.CreateWeatherCard
			
			// TODO Bonus : Display a message when the temperature is very cold and when it's very hot
			// Hint -> You can choose your hot/cold thresholds :)

			return await stepContext.EndDialogAsync(null, cancellationToken);
		}
	}
}