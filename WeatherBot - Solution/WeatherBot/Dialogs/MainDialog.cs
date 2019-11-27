using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using WeatherBot.Services.WeatherStack;
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
			var luisResult =
				await _botServices.LuisService.RecognizeAsync<WeatherLuis>(stepContext.Context, cancellationToken);
			var qnaResult = await _botServices.QnAMakerService.GetAnswersAsync(stepContext.Context);

			var thresholdScore = 0.70;

			// Check if score is too low, then it is not understood.
			if (luisResult.TopIntent().score / 100 < thresholdScore &&
			    (qnaResult.FirstOrDefault()?.Score ?? 0) / 100 < thresholdScore)
			{
				var notUnderstood = "I'm sorry but I didn't understand your message. Please try to rephrase it";
				var notUnderstoodMessage = MessageFactory.Text(notUnderstood, notUnderstood, InputHints.ExpectingInput);

				return await stepContext.PromptAsync(nameof(TextPrompt),
					new PromptOptions() {Prompt = notUnderstoodMessage}, cancellationToken);
			}

			// Check on scores between Luis and Qna.
			if (luisResult.TopIntent().score >= (qnaResult.FirstOrDefault()?.Score ?? 0))
			{
				switch (luisResult.TopIntent().intent)
				{
					case WeatherLuis.Intent.GetWeather:
						// Start the Luis Weather dialog.
						return await stepContext.BeginDialogAsync(nameof(LuisWeatherDialog), luisResult, cancellationToken);
					
					default:
						// Display a not understood message.
						var notUnderstood = "I'm sorry but I didn't understand your message. Please try to rephrase it";
						var notUnderstoodMessage =
							MessageFactory.Text(notUnderstood, notUnderstood, InputHints.ExpectingInput);
						return await stepContext.PromptAsync(nameof(TextPrompt),
							new PromptOptions() {Prompt = notUnderstoodMessage}, cancellationToken);
				}
			}

			// Show a Qna message.
			var qnaMessage = MessageFactory.Text(qnaResult.First().Answer, qnaResult.First().Answer,
				InputHints.ExpectingInput);
			return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions() {Prompt = qnaMessage},
				cancellationToken);
		}

		private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			// Check that we ended the LUIS dialog.
			if (stepContext.Result != null && stepContext.Result is WeatherDetails)
			{
				// End the dialog by replacing the current one with the root one.
				// We also pass the sentence that we will display in the initial step dialog.
				var msg = "What else can I do for you?";
				return await stepContext.ReplaceDialogAsync(InitialDialogId, msg, cancellationToken);
			}

			return await stepContext.ReplaceDialogAsync(InitialDialogId, cancellationToken: cancellationToken);
		}
	}
}