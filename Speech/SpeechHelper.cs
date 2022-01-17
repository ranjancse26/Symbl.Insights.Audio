using NAudio.Wave;
using NAudio.Lame;
using System.IO;
using System.Speech.Synthesis;

namespace Symbl.Insights.Audio.Speech
{
    public class VoiceHint
    {
        public VoiceGender VoiceGender { get; set; }
        public VoiceAge VoiceAge { get; set; }
        public int Age { get; set; }
    }

    public class SpeechHelper
    {
        public static void ConvertToAudio(string content, 
            string audioFileName, VoiceHint voiceHint)
        {
            System.Console.WriteLine("Running the text to speech convertor");

            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            {              
                speechSynthesizer.SelectVoiceByHints(voiceHint.VoiceGender,
                            voiceHint.VoiceAge, voiceHint.Age,
                            System.Globalization.CultureInfo.CurrentCulture);

                speechSynthesizer.Volume = 100;
                speechSynthesizer.Rate = 0; 

                MemoryStream memoryStream = new MemoryStream();
                speechSynthesizer.SetOutputToWaveStream(memoryStream);

                //do speaking
                speechSynthesizer.Speak(content);

                ConvertWavStreamToMp3File(memoryStream, audioFileName);
            }

            System.Console.WriteLine("Completed running the text to speech convertor");
        }

        private static void ConvertWavStreamToMp3File(MemoryStream ms,
           string fileToSave)
        {
            //rewind to beginning of stream
            ms.Seek(0, SeekOrigin.Begin);

            using (var memoryStream = new MemoryStream())
            using (var waveFileReader = new WaveFileReader(ms))
            using (var mp3FileWriter = new LameMP3FileWriter(fileToSave,
                waveFileReader.WaveFormat, LAMEPreset.VBR_90))
            {
                waveFileReader.CopyTo(mp3FileWriter);
            }
        }
    }
}
