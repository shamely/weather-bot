using System;
using System.Collections.Generic;
using AdaptiveCards;
using Microsoft.Bot.Schema;
using WeatherBot.WeatherStack;

namespace WeatherBot.Cards
{
	public class WeatherCards
	{
		public static Attachment CreateWeatherCard(WeatherDetails weatherDetails)
		{
			var card = new AdaptiveCard("1.0");

			// Intro of the card contains the Place name in large text, subtle for a grey look and feel
			card.Body.Add(new AdaptiveTextBlock
			{
				Text = weatherDetails?.PlaceName ?? "Unknown",
				Size = AdaptiveTextSize.Large,
				IsSubtle = true
			});

			// Below, without space, the date as in "Day of the week, day Month year"
			card.Body.Add(new AdaptiveTextBlock
			{
				Text = weatherDetails?.ForecastDate.ToString("dddd, dd MMM yyyy") ?? "Unknown",
				Spacing = AdaptiveSpacing.None
			});

			// To display the info next to each other below, we need to use columns
			//		We'll add the columns to the card's body later
			var columnSet = new AdaptiveColumnSet
			{
				Columns = new List<AdaptiveColumn>()
			};

			// The first column width is going to be automatic => it is the size of what it contains
			columnSet.Columns.Add(new AdaptiveColumn
			{
				Width = AdaptiveColumnWidth.Auto,
				Items = new List<AdaptiveElement>
				{
					// The first element in the column is an image (the forecast icon url)
					//		and we put it to small size so it's not too big on the card
					new AdaptiveImage
					{
						Url = new Uri(weatherDetails?.WeatherIconUrl),
						Size = AdaptiveImageSize.Small
					}
				}
			});

			// Next to the 1st column, we add another column containing some text for the temperature
			columnSet.Columns.Add(new AdaptiveColumn
			{
				Width = AdaptiveColumnWidth.Auto,
				Items = new List<AdaptiveElement>
				{
					// We want the temperature text to be super large so it pops out
					new AdaptiveTextBlock
					{
						Text = weatherDetails?.CurrentTemp.ToString() ?? "Unknown",
						Size = AdaptiveTextSize.ExtraLarge,
						Spacing = AdaptiveSpacing.None
					}
				}
			});

			// To add the little Celsius sign near the temperature, but not as big, we had to add another column
			//		We want this column width to stretch to display the min and max temperatures in the 4th column
			//		on the far right of the card
			columnSet.Columns.Add(new AdaptiveColumn
			{
				Width = AdaptiveColumnWidth.Stretch,
				Items = new List<AdaptiveElement>
				{
					// We want to Celsius text to be a bit bold for styling reasons
					new AdaptiveTextBlock
					{
						Text = "°C",
						Weight = AdaptiveTextWeight.Bolder,
						Spacing = AdaptiveSpacing.Small
					}
				}
			});

			// In this column, we're going to display the min and max temperature for the forecast of the day.
			//		Since they are on top of each other in my designs, we put them in the same column. Otherwise
			//		we would have added a column for each.
			columnSet.Columns.Add(new AdaptiveColumn
			{
				Width = AdaptiveColumnWidth.Stretch,
				Items = new List<AdaptiveElement>
				{
					// We align the text to the left of its container
					new AdaptiveTextBlock
					{
						Text = $"Max {weatherDetails?.MaxTemp}",
						HorizontalAlignment = AdaptiveHorizontalAlignment.Left
					},
					new AdaptiveTextBlock
					{
						Text = $"Min {weatherDetails?.MinTemp}",
						HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
						Spacing = AdaptiveSpacing.None
					}
				}
			});

			card.Body.Add(columnSet);

			// Transform the card into an attachment which is the correct format to attach it into an activity
			return new Attachment(AdaptiveCard.ContentType, content: card);
		}
	}
}