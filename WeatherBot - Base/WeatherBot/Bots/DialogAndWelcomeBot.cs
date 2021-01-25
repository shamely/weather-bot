// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.11.1

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherBot.Bots
{
	public class DialogAndWelcomeBot<T> : DialogBot<T>
		where T : Dialog
	{
		private const string GreetingMsg = "Hello, I'm the weather chatbot. I can predict the weather! 🌤";
		private const string LetsStartMsg =
			"Let's start chatting together! You can start by asking me about the weather" +
			"where you live or in your favorite destination 🗺";

		public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
			: base(conversationState, userState, dialog, logger)
		{
		}

		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
		{
			foreach (var member in membersAdded)
			{
				// Greet anyone that was not the target (recipient) of this message.
				// To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
				if (member.Id != turnContext.Activity.Recipient.Id)
				{
					//TODO: greet the user with the 2 messages;
					// HINT -> use MessageFactory (static class) to create the message
					// HINT -> use the context and find the send activity method

					await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
				}
			}
		}
	}
}

