using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using WeatherLuis = WeatherBot.CognitiveModels.WeatherLuis;

namespace WeatherBot.Dialogs
{
	public class MainDialog : ComponentDialog
	{
		private readonly IBotServices _botServices;
		protected readonly ILogger Logger;

		public MainDialog(IBotServices botServices, ILogger<MainDialog> logger) :
			base(nameof(MainDialog))
		{
			_botServices = botServices;
			Logger = logger;

			// Register all the dialogs that will be called (Prompts, LuisWeather, Waterfall Steps).
			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new LuisWeatherDialog(nameof(LuisWeatherDialog), botServices));

			// Define the steps for the waterfall dialog.
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				IntroStepAsync,
				DispatchStepAsync,
				FinalStepAsync
			}));

			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			// Check if message from the final step present. If so, display it as Prompt, else skip to next step.
			var greeting = stepContext.Options?.ToString();
			if (string.IsNullOrEmpty(greeting))
			{
				return await stepContext.NextAsync(null, cancellationToken);
			}

			var promptText = MessageFactory.Text(greeting, greeting, InputHints.ExpectingInput);
			return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions() { Prompt = promptText },
				cancellationToken);
		}

		private async Task<DialogTurnResult> DispatchStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			// Call the services to dispatch to the correct dialog.

			// Check on scores for Luis. We could do the same for Qna and compete the scores
			// If Luis intent recognized, launch the dialog. Else display a not understood message.

			// End this step in case of no dialog launched.
			return await stepContext.NextAsync(null, cancellationToken);
		}

		private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			// Check if success from the last dialog (to differentiate if not understood or luis).

			// End the dialog by replacing the current one with the root one.
			// We also pass the sentence that we will display in the initial step dialog.
		}
	}
}