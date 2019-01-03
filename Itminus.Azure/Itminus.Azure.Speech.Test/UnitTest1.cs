using System;
using Xunit;
using Itminus.Azure.Speech;
using System.IO;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;

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
        public void TestText2SpeechService()
        {
            var speechService = new Itminus.Azure.Speech.SpeechServiceBuilder()
                .SetRegion(this.Region)
                .SetSubScriptionKey(this.Key)
                .Build();
            var stream = speechService.FetchSpeechAsync("hello,world").Result;
            Assert.NotNull(stream);
            var saveAs = Path.Combine(Directory.GetCurrentDirectory(),@"test-text-to-speech.wav");
            using (var fs = File.OpenWrite(saveAs)) {
                stream.CopyTo(fs);
            } 
            Assert.True(File.Exists(saveAs));
            File.Delete(saveAs);
        }

    }
}
