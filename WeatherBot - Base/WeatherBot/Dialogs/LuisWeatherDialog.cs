using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using WeatherBot.Services.WeatherStack;
using WeatherLuis = WeatherBot.CognitiveModels.WeatherLuis;

namespace WeatherBot.Dialogs
{
	public class LuisWeatherDialog : ComponentDialog
	{
		private readonly IBotServices _botServices;

		public LuisWeatherDialog(string id, IBotServices botServices) : base(id)
		{
			_botServices = botServices;

			// Add the dialogs.
			
		}

		private async Task<DialogTurnResult> GetPlaceStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			// Grab Luis result from the options

			// Check to get the entities from the Luis result
			// If not null, next step with the result, else ask to the user to give us the place name.
		}

		private async Task<DialogTurnResult> ShowWeatherStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			// Fetch the place from the context, either from the previous dialog, either from the user input.

			// Fetch the forecast details. If null, display that something went wrong.

			// Else, reply with a card as an attachment.

			// End the dialog
			return await stepContext.EndDialogAsync(null, cancellationToken);
		}

		// This method is to create the weather adaptive card.
		private static Attachment CreateWeatherCard(WeatherDetails weatherDetails)
		{
			var card = new AdaptiveCard("1.0");

			card.Body.Add(new AdaptiveTextBlock()
			{
				Text = weatherDetails?.PlaceName ?? "Unknown",
				Size = AdaptiveTextSize.Large,
				IsSubtle = true
			});

			card.Body.Add(new AdaptiveTextBlock()
			{
				Text = weatherDetails?.ForecastDate.ToString("dddd, dd MMMM yyyy") ?? "Unknown",
				Spacing = AdaptiveSpacing.None
			});

			var columnSet = new AdaptiveColumnSet()
			{
				Columns = new List<AdaptiveColumn>()
			};

			columnSet.Columns.Add(new AdaptiveColumn()
			{
				Width = AdaptiveColumnWidth.Auto,
				Items = new List<AdaptiveElement>()
				{
					new AdaptiveImage()
					{
						Url = new Uri(weatherDetails?.WeatherIconUrl),
						Size = AdaptiveImageSize.Small
					}
				}
			});

			columnSet.Columns.Add(new AdaptiveColumn()
			{
				Width = AdaptiveColumnWidth.Auto,
				Items = new List<AdaptiveElement>()
				{
					new AdaptiveTextBlock()
					{
						Text = weatherDetails?.CurrentTemp.ToString() ?? "Unknown",
						Size = AdaptiveTextSize.ExtraLarge,
						Spacing = AdaptiveSpacing.None
					}
				}
			});

			columnSet.Columns.Add(new AdaptiveColumn()
			{
				Width = AdaptiveColumnWidth.Stretch,
				Items = new List<AdaptiveElement>()
				{
					new AdaptiveTextBlock()
					{
						Text = "°C",
						Weight = AdaptiveTextWeight.Bolder,
						Spacing = AdaptiveSpacing.Small
					}
				}
			});

			columnSet.Columns.Add(new AdaptiveColumn()
			{
				Width = AdaptiveColumnWidth.Stretch,
				Items = new List<AdaptiveElement>()
				{
					new AdaptiveTextBlock()
					{
						Text = $"Max {weatherDetails?.MaxTemp}",
						HorizontalAlignment = AdaptiveHorizontalAlignment.Left
					},
					new AdaptiveTextBlock()
					{
						Text = $"Min {weatherDetails?.MinTemp}",
						HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
						Spacing = AdaptiveSpacing.None
					}
				}
			});

			card.Body.Add(columnSet);

			return new Attachment(AdaptiveCard.ContentType, content: card);
		}
	}
}
