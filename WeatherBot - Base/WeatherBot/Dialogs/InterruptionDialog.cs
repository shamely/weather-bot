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

				// TODO Test for LUIS
				// Hint -> use the WeatherRecognizer and the Weather type.

				// TODO compare the intent score with our Threshold
				// Hint -> If lower, call the Not Understood dialog
				// Hint -> Switch on the intent type. If Intent = GetWeather, call the Weather Dialog
				// Hint -> If Intent is default (None), call the Not Understood dialog
				// Hint -> To call a dialog, use the context and find a method BeginDialogAsync
			}

			return new DialogTurnResult(DialogTurnStatus.Waiting);
		}
	}
}