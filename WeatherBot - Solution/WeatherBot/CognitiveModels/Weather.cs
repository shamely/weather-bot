using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Newtonsoft.Json;

namespace WeatherBot.CognitiveModels
{
	// File generated with the LUISGen tool - Check it up on Github: https://github.com/microsoft/botbuilder-tools/tree/master/packages/LUISGen
	public partial class Weather : IRecognizerConvert
	{
		[JsonProperty("text")]
		public string Text;

		[JsonProperty("alteredText")]
		public string AlteredText;

		public enum Intent
		{
			GetWeather,
			None
		};

		[JsonProperty("intents")]
		public Dictionary<Intent, IntentScore> Intents;

		public class _Entities
		{
			// Simple entities
			public string[] Places;

			// Built-in entities
			public GeographyV2[] geographyV2;

			// Instance
			public class _Instance
			{
				public InstanceData[] Places;
				public InstanceData[] geographyV2;
			}
			[JsonProperty("$instance")]
			public _Instance _instance;
		}

		[JsonProperty("entities")]
		public _Entities Entities;

		[JsonExtensionData(ReadData = true, WriteData = true)]
		public IDictionary<string, object> Properties { get; set; }

		public void Convert(dynamic result)
		{
			var app = JsonConvert.DeserializeObject<Weather>(JsonConvert.SerializeObject(result,
				new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
			Text = app.Text;
			AlteredText = app.AlteredText;
			Intents = app.Intents;
			Entities = app.Entities;
			Properties = app.Properties;
		}

		public (Intent intent, double score) TopIntent()
		{
			var maxIntent = Intent.None;
			var max = 0.0;

			foreach (var (intentName, intentScore) in Intents)
			{
				if (intentScore.Score > max)
				{
					maxIntent = intentName;
					max = intentScore.Score.Value;
				}
			}
			return (maxIntent, max);
		}
	}
}