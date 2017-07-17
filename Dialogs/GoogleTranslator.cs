using System;
using System.Net;

namespace CrownberryBot.Dialogs
{
    public class GoogleTranslator
    {
        private const string GoogleUrl = "http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}";

        public string TranslateText(string input, string languagePair)
        {
            var url = string.Format(GoogleUrl, input, languagePair);
            var webClient = new WebClient {Encoding = System.Text.Encoding.UTF8};
            var result = webClient.DownloadString(url);
            result = result.Substring(result.IndexOf("id=result_box") + 22, result.IndexOf("id=result_box") + 500);
            result = result.Substring(0, result.IndexOf("</div"));
            return result;
        }
    }
}