// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace WeatherBot.Bots
{
	// This IBot implementation can run any type of Dialog.
	// The use of type parametrization is to allows multiple
	// different bots to be run at different endpoints within the same project.
	public class WeatherBot<T> : ActivityHandler where T: Dialog
	{
		protected ILogger Logger;
		protected readonly IBotServices BotServices;

		protected readonly Dialog Dialog;

		// State management.
		protected readonly BotState ConversationState;
		protected readonly BotState UserState;

		public WeatherBot(IBotServices botServices, ILogger<WeatherBot<T>> logger, T dialog,
			ConversationState conversationState, UserState userState)
		{
			Logger = logger;
			BotServices = botServices;

			Dialog = dialog;

			ConversationState = conversationState;
			UserState = userState;
		}

		// Method called on each turn. You can either dispatch from the
		// activity type here or use the OnActivityTypeAsync like below.
		public override async Task OnTurnAsync(ITurnContext turnContext,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			await base.OnTurnAsync(turnContext, cancellationToken);

			// Save any state changes that might have occured during the turn.
			await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
			await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
		}

		// Whenever the activity is of type "message".
		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, 
			CancellationToken cancellationToken)
		{
			Logger.LogInformation("Running dialog with Message Activity");

			// Run the last dialog in the stack.
			await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)),
				cancellationToken);
		}

		// Whenever the activity of "On member added" -> "handshake".
		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, 
			ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
		{
			foreach (var member in membersAdded)
			{
				// If the member added isn't the bot.
				if (member.Id != turnContext.Activity.Recipient.Id)
				{
					await turnContext.SendActivityAsync(
						"Hello, I am the weather bot. How can I help you today?",
						cancellationToken: cancellationToken);
				}
			}
		}
	}
}
