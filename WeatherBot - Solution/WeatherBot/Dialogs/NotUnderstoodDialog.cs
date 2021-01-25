using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace WeatherBot.Dialogs
{
	public class NotUnderstoodDialog : ComponentDialog
	{
		private const string NotUnderstoodMsgText = "I'm sorry I didn't get what you were saying 😳...";
		public NotUnderstoodDialog(string id) : base(id) { }

		protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options,
			CancellationToken cancellationToken = new CancellationToken())
		{
			var notUnderstoodMessage = MessageFactory.Text(NotUnderstoodMsgText, NotUnderstoodMsgText);
			await innerDc.Context.SendActivityAsync(notUnderstoodMessage, cancellationToken);

			return await innerDc.EndDialogAsync(null, cancellationToken);
		}
	}
}