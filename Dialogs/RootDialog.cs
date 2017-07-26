using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Chronic.Handlers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace CrownberryBot.Dialogs
{

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string ResponseString = "1 BTC = {0} USD \n\n1 ETH = {1} USD";

        private static readonly string[] BtcMatches =
        {
            "битк", "биток",
            "майнить", "майним", "майнер",
            "видюх", "видеокарт",
            "btc", "bitcoin"
        };

        private static readonly string[] NoMathces =
        {
            "нет "
        };

        private static readonly string[] PatMathces =
        {
            "жаль "
        };


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
                if (activity?.Text != null)
                {
                    var culture = CultureInfo.GetCultureInfo("ru-RU");
                    var textList = activity.Text.Split();
                    switch (textList[0])
                    {
                        case "/help":
                        case "/help@crownberry_bot":
                            await context.PostAsync(
                                $"Hello, {activity.From.Name}. Type \"btc\" if u wanna know current BTC-USD rate.");
                            break;
                        default:
                            if (BtcMatches.Any(e => culture
                                                        .CompareInfo
                                                        .IndexOf(activity.Text, e, CompareOptions.IgnoreCase) >= 0))
                            {
                                var btc = CoinRate.GetRate("btc");
                                var eth = CoinRate.GetRate("eth");
                                var resp = string.Format(ResponseString, btc, eth);
                                await context.PostAsync(resp);
                            }
                            else if (NoMathces.Any(e => culture
                                                         .CompareInfo
                                                         .IndexOf(activity.Text, e, CompareOptions.IgnoreCase) >= 0))
                            {
                                var resp = "Догадайся кого ответ KappaPride";
                                await context.PostAsync(resp);
                            }
                            else if (PatMathces.Any(e => culture
                                                         .CompareInfo
                                                         .IndexOf(activity.Text, e, CompareOptions.IgnoreCase) >= 0))
                            {
                                var resp = "И мне тебя жаль" +
                                           "";
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