using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace Speechtotext
{
    internal class Program
    {
        static async Task Main()
        {
            await SynthesizeAudioAsync();
        }

        private static async Task SynthesizeAudioAsync()
        {
            var config = SpeechConfig.FromSubscription("", "");
           

            using (var recog= new SpeechRecognizer(config))
            {
                Console.WriteLine("Speak");

                var result = await recog.RecognizeOnceAsync();

                if(result.Reason ==ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine(result.Text);
                }

                config.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
                using var synthesizer = new SpeechSynthesizer(config);
                Console.WriteLine("Your robot will work now");
                await synthesizer.SpeakTextAsync(result.Text);
            }


            
        }
    }
}
