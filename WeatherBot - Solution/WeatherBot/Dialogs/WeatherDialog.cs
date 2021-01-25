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
		private readonly BotState _conversationState;

		private const string GetPlaceMsgText = "Could you tell me from which city you'd like to get the weather? 🌞";
		private const string RecapPlaceMsgText = "I understood you want the weather from: ";
		private const string SomethingWentWrongMsgText = "Something went wrong while fetching the weather forecast... 😞";
		private const string ColdWeatherMsgTxt = "Brrr 🥶, looks like it's cold today. Better wear a hot sweater 🧶";
		private const string HotWeatherMsgTxt = "Mhhh, I love a nice warm weather ☀️😎. Don't you?";

		public WeatherDialog(WeatherStackApiServices weatherStackApiServices, BotState conversationState, string id) : base(id)
		{
			_weatherStackApiServices = weatherStackApiServices;
			_conversationState = conversationState;

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
			_luisResult = (Weather) stepContext.Options;

			if (string.IsNullOrEmpty(_luisResult.PlaceEntities))
			{
				var getPlaceMessage = MessageFactory.Text(GetPlaceMsgText, GetPlaceMsgText, InputHints.ExpectingInput);
				await stepContext.Context.SendActivityAsync(getPlaceMessage, cancellationToken);

				var accessory = _conversationState.CreateProperty<ConversationDataState>(nameof(ConversationDataState));
				var conversationData = await accessory.GetAsync(stepContext.Context, () => new ConversationDataState(),
					cancellationToken);
				conversationData.IgnoreInterruption = true;
				await accessory.SetAsync(stepContext.Context, conversationData, cancellationToken);

				return new DialogTurnResult(DialogTurnStatus.Waiting);
			}

			// Skip the wait and go directly to next step.
			return await stepContext.NextAsync(_luisResult.PlaceEntities, cancellationToken);
		}

		private async Task<DialogTurnResult> ShowWeatherAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			var place = (string) stepContext.Result;
			var recapMsg = RecapPlaceMsgText + place;
			
			var recapMessage = MessageFactory.Text(recapMsg, recapMsg, InputHints.IgnoringInput);
			await stepContext.Context.SendActivityAsync(recapMessage, cancellationToken);

			var weatherDetails = await _weatherStackApiServices.GetForecastWeather(place);

			if (weatherDetails == null)
			{
				var somethingWentWrongMessage =
					MessageFactory.Text(SomethingWentWrongMsgText, SomethingWentWrongMsgText);
				await stepContext.Context.SendActivityAsync(somethingWentWrongMessage, cancellationToken);

				return await stepContext.EndDialogAsync(null, cancellationToken);
			}

			var weatherCard = MessageFactory.Attachment(WeatherCards.CreateWeatherCard(weatherDetails));
			await stepContext.Context.SendActivityAsync(weatherCard, cancellationToken);

			if (weatherDetails.CurrentTemp < 0)
			{
				var endingMsg = MessageFactory.Text(ColdWeatherMsgTxt, ColdWeatherMsgTxt);
				await stepContext.Context.SendActivityAsync(endingMsg, cancellationToken);
			} 
			else if (weatherDetails.CurrentTemp > 20)
			{
				var endingMsg = MessageFactory.Text(HotWeatherMsgTxt, HotWeatherMsgTxt);
				await stepContext.Context.SendActivityAsync(endingMsg, cancellationToken);
			}

			return await stepContext.EndDialogAsync(null, cancellationToken);
		}
	}
}