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
                       await synthesizer.SpeakTextAsync(result.Text);
                        Console.WriteLine($"Final Statement: {result.Text} ");
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

                do { Console.Write("Press enter to stop"); }
                while(Console.ReadKey().Key!= ConsoleKey.Enter);

                await recog.StopContinuousRecognitionAsync().ConfigureAwait(false);


            }


            
        }
    }
}
