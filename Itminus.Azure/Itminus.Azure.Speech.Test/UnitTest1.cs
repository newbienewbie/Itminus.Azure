using System;
using Xunit;
using Itminus.Azure.Speech;
using System.IO;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Itminus.Azure.Speech.Test
{
    public class UnitTest1
    {

        public IConfiguration Configuration{get;set;}
        public string Region { get; }
        public string Key { get; }

        public UnitTest1()
        {
            this.Configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("azure.config.json")
                .AddJsonFile("azure.test.config.json")
                .Build();
            this.Region = this.Configuration.GetSection("region").Value;
            this.Key = this.Configuration.GetSection("key").Value;
        }

        [Fact]
        public void TestSpeechTokenAccessor()
        {
            var tokenAccessor= new SpeechServiceBuilder()
                .SetRegion(this.Region)
                .SetSubScriptionKey(this.Key)
                .BuildSpeechTokenAccessor();
            Assert.NotNull(tokenAccessor.Token);
        }

        [Fact]
        public async Task TestText2SpeechService()
        {
            var speechService = new Itminus.Azure.Speech.SpeechServiceBuilder()
                .SetRegion(this.Region)
                .SetSubScriptionKey(this.Key)
                .Build();
            var stream =await speechService.FetchSpeechAsync("hello,world");
            Assert.NotNull(stream);
            var saveAs = Path.Combine(Directory.GetCurrentDirectory(),@"test-text-to-speech.wav");
            using (var fs = File.OpenWrite(saveAs)) {
                stream.CopyTo(fs);
            } 
            Assert.True(File.Exists(saveAs));
            File.Delete(saveAs);
        }


        [Theory]
        [InlineData("en-US", "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)", "It works")]
        [InlineData("zh-CN", "Microsoft Server Speech Text to Speech Voice (zh-CN, Yaoyao, Apollo)", "学习雷锋好榜样")]
        [InlineData("zh-CN", "Microsoft Server Speech Text to Speech Voice (zh-CN, HuihuiRUS)", "忠于人民忠于党")]
        public async Task TestSpeech2TextService(string lang,string voiceName, string text)
        {
            var speechService = new SpeechServiceBuilder()
                .SetRegion(this.Region)
                .SetSubScriptionKey(this.Key)
                .Build();

            var inputStream = await speechService.FetchSpeechAsync(text,voiceName);
            var contentType = "audio/wav; codecs=audio/pcm; samplerate=16000";
            var outStream =await speechService.FetchTextAsStreamResultAsync(inputStream,contentType,lang);
            Assert.NotNull(outStream);
            using (var reader = new JsonTextReader(new StreamReader(outStream))) {
                var o = (JObject) JToken.ReadFrom(reader);
                Assert.Equal("Success", o["RecognitionStatus"].Value<string>());
                Assert.Contains(text,o["DisplayText"].Value<string>());
            } 
        }
    }
}
