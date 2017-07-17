using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System;

namespace CrownberryBot.Dialogs
{
    internal class BitcoinRate
    {
        private const string url = "https://api.cryptonator.com/api/ticker/";
        private const string responseString = "Сейчас один биток стоит {0} долларов";

        public static string GetBTC(string cur)
        {
            var request = (HttpWebRequest) WebRequest.Create(url+cur+"-usd");
            var response = request.GetResponse();
            var sr = new StreamReader(response.GetResponseStream());
            var jsonText = sr.ReadToEnd();
            var jObj = JObject.Parse(jsonText);
            var ticker = (JObject)jObj["ticker"];
            var nowRate = ticker["price"].ToString();
            var fltRate = Math.Round(float.Parse(nowRate, CultureInfo.InvariantCulture.NumberFormat), 2);
            return fltRate.ToString();
        }
    }
}