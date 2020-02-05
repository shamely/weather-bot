using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Newtonsoft.Json;

namespace WeatherBot.CognitiveModels
{
	/// <summary>
	/// Convert the generic recognizer that is used for Luis entities and intents into
	/// a strongly typed one. This helps us for the entities recognition and use in the code.
	/// </summary>
	public partial class WeatherLuis : IRecognizerConvert
	{
		public string Text;
		public string AlteredText;

		public enum Intent
		{
			GetWeather,
			None
		};

		public Dictionary<Intent, IntentScore> Intents;

		public class _Entities
		{
			// Pre-built entity.
			public GeographyV2 geography;

			// Composite.
			public class _InstancePlace
			{
				public InstanceData[] geography;
			}

			public class PlaceClass
			{
				public GeographyV2 geography;
				[JsonProperty("$instance")] public _InstancePlace _instance;
			}

			public PlaceClass[] Place;

			// Instance.
			public class _Instance
			{
				public InstanceData[] geography;
				public InstanceData[] Place;
			}
			[JsonProperty("$instance")] public _Instance _instance;
		}

		public _Entities Entities;

		[JsonExtensionData(ReadData = true, WriteData = true)]
		public IDictionary<string, object> Properties { get; set; }

		public void Convert(dynamic result)
		{
			var app = JsonConvert.DeserializeObject<WeatherLuis>(JsonConvert.SerializeObject(result,
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
			foreach (var entry in Intents)
			{
				if (entry.Value.Score > max)
				{
					maxIntent = entry.Key;
					max = entry.Value.Score.Value;
				}
			}
			return (maxIntent, max);
		}
	}
}