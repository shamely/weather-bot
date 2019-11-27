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
			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				GetPlaceStepAsync,
				ShowWeatherStepAsync
			}));

			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> GetPlaceStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			var luisResult = (WeatherLuis) stepContext.Options;

			if (string.IsNullOrEmpty(luisResult.PlaceEntities))
			{
				if (string.IsNullOrEmpty(luisResult.GeographyEntities))
				{
					var retry = "From which city would you like to see the weather?";
					var retryMessage = MessageFactory.Text(retry, retry, InputHints.ExpectingInput);

					return await stepContext.PromptAsync(nameof(TextPrompt),
						new PromptOptions() {Prompt = retryMessage}, cancellationToken);
				}

				return await stepContext.NextAsync(luisResult.GeographyEntities, cancellationToken);
			}

			return await stepContext.NextAsync(luisResult.PlaceEntities, cancellationToken);
		}

		private async Task<DialogTurnResult> ShowWeatherStepAsync(WaterfallStepContext stepContext,
			CancellationToken cancellationToken)
		{
			var weatherPlace = (string) stepContext.Result;

			var weatherDetails = await _botServices.WeatherStackServices.GetForecastWeather(weatherPlace);
			if (weatherDetails == null)
			{
				var somethingWentWrong = "Something went wrong while fetching the weather.";
				var wrongMessage =
					MessageFactory.Text(somethingWentWrong, somethingWentWrong, InputHints.IgnoringInput);
				await stepContext.Context.SendActivityAsync(wrongMessage, cancellationToken);

				
			}
			else
			{
				var weatherCardMessage = MessageFactory.Attachment(CreateWeatherCard(weatherDetails));
				await stepContext.Context.SendActivityAsync(weatherCardMessage, cancellationToken);
			}

			return await stepContext.EndDialogAsync(weatherDetails, cancellationToken);
		}

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
