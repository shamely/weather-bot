// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.11.1

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using WeatherBot.CognitiveModels;
using WeatherBot.Recognizers;
using WeatherBot.WeatherStack;

namespace WeatherBot.Dialogs
{
	public class MainDialog : InterruptionDialog
	{
		// Dependency injection uses this constructor to instantiate MainDialog
		public MainDialog(WeatherRecognizer luisRecognizer, WeatherStackApiServices weatherStackApiServices, ConversationState conversationState)
			: base(luisRecognizer, weatherStackApiServices, conversationState, nameof(MainDialog)) { }
	}
}
