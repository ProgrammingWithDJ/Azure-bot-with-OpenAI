using System;
using System.IO;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json;

namespace Speechtotext
{
    internal class Program
    {
        static async Task Main()
        {
            await SynthesizeAudioAsync();
        }

        public static string openAI(string inputText)
        {
            var apiKey = "";
            var model = "text-davinci-003";
            var prompt = inputText;
            var maxTokens = 100;
            var temperature = 0;

            var request = (HttpWebRequest)WebRequest.Create("https://api.openai.com/v1/completions");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + apiKey);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"model\":\"" + model + "\",\"prompt\":\"" + prompt + "\",\"max_tokens\":" + maxTokens + ",\"temperature\":" + temperature + "}";
                streamWriter.Write(json);
            }

            var response = (HttpWebResponse)request.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();

                dynamic jsonObj = JsonConvert.DeserializeObject(result);
                string text = jsonObj.choices[0].text;
                return text;
            }

            return "Someting failed";

        }

        private static async Task SynthesizeAudioAsync()
        {
            var config = SpeechConfig.FromSubscription("44a11e635d74467c85b5846aea9c7c0d", "eastus");
           

            using (var recog= new SpeechRecognizer(config))
            {
                Console.WriteLine("Speak");

                recog.Recognizing += (sender, eventArgs) =>
                {
                    Console.WriteLine($"Recognizing: {eventArgs.Result.Text}");
                };

                recog.Recognized += async (sender, eventArgs) =>
                {
                    var result = eventArgs.Result;

                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {

                        config.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
                        using var synthesizer = new SpeechSynthesizer(config);

                        Console.WriteLine("Your robot will work now");
                       
                      //  Console.WriteLine($"Final Statement: {result.Text} ");

                        var ans =openAI(result.Text);
                        await synthesizer.SpeakTextAsync(ans);

                    }


                };

                recog.SessionStarted += (sender, eventArgs) =>
                {
                    Console.WriteLine("Session started,, you can start speaking");
                };


                recog.SessionStopped += (sender, eventArgs) =>
                {
                    Console.WriteLine("SessionStopped");
                };

                await recog.StartContinuousRecognitionAsync().ConfigureAwait(false);

                do { Console.WriteLine("Press enter to stop"); }
                while(Console.ReadKey().Key!= ConsoleKey.Enter);

                await recog.StopContinuousRecognitionAsync().ConfigureAwait(false);


            }


            
        }
    }
}
