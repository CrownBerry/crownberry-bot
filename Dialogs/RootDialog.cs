﻿using System;
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
    public class RootDialog : LuisDialog<object>
    {
        private const string ResponseString = "1 BTC = {0} USD \n\n1 ETH = {1} USD";

        private static readonly string[] BtcMatches =
        {
            "битк", "биток",
            "майнить", "майним", "майнер",
            "видюх", "видеокарт",
            "btc", "bitcoin"
        };

        public RootDialog() : base(new LuisService(new LuisModelAttribute("605c612f-5aef-4c13-9149-9d9b41627ec5",
            "4e3011e930d94975a588111368fbdfff")))
        {

        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        [LuisIntent("NoneIntent")]
        public async Task PlacesTaskAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"{result.Intents[0]}");
            context.Wait(MessageReceived);
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
                                $"Hello, {activity.From.Name}. Type \"btc\" if u want to know current BTC-USD rate.");
                            break;
                        case "/weather":
                        case "/weather@crownberry_bot":
                            //var isCity = LuisParser.GetCity(textList[1]);
                            var fullJson = LuisParser.GetFullJson(textList[1]);
                            await context.PostAsync(fullJson);
                            //if (isCity != "No city")
                            //    await context.PostAsync($"Вы упомянули город {isCity}. Скоро я научусь вам говорить погоду в этом городе");
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
                            var textJson = LuisParser.GetFullJson(textList[1]);
                            await context.PostAsync(textJson);
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