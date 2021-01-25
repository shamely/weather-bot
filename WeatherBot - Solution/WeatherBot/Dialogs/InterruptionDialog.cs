using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using WeatherBot.CognitiveModels;
using WeatherBot.Recognizers;
using WeatherBot.WeatherStack;

namespace WeatherBot.Dialogs
{
	public class InterruptionDialog : ComponentDialog
	{
		protected readonly WeatherRecognizer LuisRecognizer;
		protected readonly WeatherStackApiServices WeatherStackApiServices;
		private const double LuisThresholdScore = 0.70;

		public InterruptionDialog(WeatherRecognizer luisRecognizer, WeatherStackApiServices weatherStackApiServices, string id) : base(id)
		{
			LuisRecognizer = luisRecognizer;
			WeatherStackApiServices = weatherStackApiServices;

			AddDialog(new WeatherDialog(weatherStackApiServices, nameof(WeatherDialog)));
			AddDialog(new NotUnderstoodDialog(nameof(NotUnderstoodDialog)));
		}

		protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options,
			CancellationToken cancellationToken = new CancellationToken())
		{
			var result = await InterruptAsync(innerDc, cancellationToken);

			if (result != null)
				return result;
			return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
		}

		protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc,
			CancellationToken cancellationToken = default)
		{
			var result = await InterruptAsync(innerDc, cancellationToken);

			if (result != null)
				return result;

			return await base.OnContinueDialogAsync(innerDc, cancellationToken);
		}

		private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
		{
			if (!string.IsNullOrEmpty(innerDc.ActiveDialog?.Id) && innerDc.ActiveDialog.Id == "WeatherDialog")
			{
				return null;
			}

			if (innerDc.Context.Activity.Type == ActivityTypes.Message)
			{
				var text = innerDc.Context.Activity.Text?.ToLowerInvariant();

				if (string.IsNullOrEmpty(text)) return null;

				// Test for LUIS
				var luisResult = await LuisRecognizer.RecognizeAsync<Weather>(innerDc.Context, cancellationToken);
				var topIntent = luisResult?.TopIntent() ?? (Weather.Intent.None, 0.0);

				// Placeholder for QnaMaker test

				// Ideally you should compare the scores from Luis and QnaMaker results when you have them
				//		and decide for which one you want to go for.

				if (topIntent.score < LuisThresholdScore)
					return await innerDc.BeginDialogAsync(nameof(NotUnderstoodDialog), cancellationToken: cancellationToken);

				switch (topIntent.intent)
				{
					case Weather.Intent.GetWeather:
						return await innerDc.BeginDialogAsync(nameof(WeatherDialog), luisResult, cancellationToken);

					default:
						return await innerDc.BeginDialogAsync(nameof(NotUnderstoodDialog), cancellationToken: cancellationToken);
				}
			}

			return new DialogTurnResult(DialogTurnStatus.Waiting);
		}
	}
}