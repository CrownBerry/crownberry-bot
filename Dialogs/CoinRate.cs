using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System;

namespace CrownberryBot.Dialogs
{
    internal class CoinRate
    {
        private const string Url = "https://api.cryptonator.com/api/ticker/";

        public static string GetRate(string cur)
        {
            var request = (HttpWebRequest) WebRequest.Create(Url+cur+"-usd");
            var response = request.GetResponse();
            var sr = new StreamReader(response.GetResponseStream());
            var jsonText = sr.ReadToEnd();
            var jObj = JObject.Parse(jsonText);
            var ticker = (JObject)jObj["ticker"];
            var nowRate = ticker["price"].ToString();
            var fltRate = Math.Round(float.Parse(nowRate, CultureInfo.CurrentCulture), 2);
            return fltRate.ToString(CultureInfo.CurrentCulture);
        }
    }
}