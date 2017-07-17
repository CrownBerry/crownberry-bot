using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Globalization;

namespace CrownberryBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private static readonly string[] btcMatches = {
            "битк", 
            "биток", 
            "майнить", "майним", 
            "майнер", 
            "видюх", 
            "видеокарт",
            "btc",
            "bitcoin"
        };
        private const string responseString = "1 BTC = {0} USD \n\n1 ETH = {1} USD";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            try
            {
                if (activity.Text != null)
                {
                    var culture = CultureInfo.GetCultureInfo("ru-RU");
                    var textList = activity.Text.Split();
                    switch (textList[0])
                    {
                        case "/help":
                            await context.PostAsync($"Hello, {activity.From.Name}. Type \"btc\" if u want to know current BTC-USD rate.");
                            break;
                        default:
                            if (btcMatches.Where(e =>
                                culture.CompareInfo.IndexOf(activity.Text, e, CompareOptions.IgnoreCase) >= 0).Any())
                            {
                                var btc = BitcoinRate.GetBTC("btc");
                                var eth = BitcoinRate.GetBTC("eth");
                                var resp = string.Format(responseString, btc, eth);
                                await context.PostAsync(resp);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                await context.PostAsync(e.ToString());
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}