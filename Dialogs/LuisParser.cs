using System;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json.Linq;

namespace CrownberryBot.Dialogs
{
    public class LuisParser
    {
        private const string LuisUrl = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/605c612f-5aef-4c13-9149-9d9b41627ec5?subscription-key=4e3011e930d94975a588111368fbdfff&timezoneOffset=0&verbose=true&q=\"{0}\"";

        public static string GetCity(string message)
        {
//            var webClient = new WebClient {Encoding = System.Text.Encoding.UTF8};
//            var result = webClient.DownloadString(string.Format(LuisUrl,message));
            var request = (HttpWebRequest) WebRequest.Create(string.Format(LuisUrl,message));
            var response = request.GetResponse();
            var sr = new StreamReader(response.GetResponseStream());
            var jsonText = sr.ReadToEnd();
            var jObject = JObject.Parse(jsonText);
            var ent = jObject["entities"];
            if (ent == null)
                return "No city";
            var cityName = ent[0]["entity"].ToString();
            return cityName;
        }

        public static string GetFullJson(string message)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(string.Format(LuisUrl, message));
                var response = request.GetResponse();
                var sr = new StreamReader(response.GetResponseStream());
                var jsonText = sr.ReadToEnd();
                var jObject = JObject.Parse(jsonText);
                return jObject.ToString();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }
    }
}