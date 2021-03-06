using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Itminus.Azure.Abstraction;
using Newtonsoft.Json;

namespace Itminus.Azure.Speech
{
    public class SpeechService{

        internal SpeechService(string textToSpeechServicePath, string speechToTextServicePath, ITokenAccessor tokenAccessor, string resourceName , SpeechOptions opts ,HttpClient client){
            this.TextToServicePath = textToSpeechServicePath;
            this.SpeechToTextPath = speechToTextServicePath;
            this.TokenAccessor=  tokenAccessor ?? throw new ArgumentNullException($"{nameof(tokenAccessor)} cannot be null");
            this.ResourceName = String.IsNullOrEmpty(resourceName) ? "YOUR_RESOURCE_NAME": resourceName;
            this.Client = client?? new HttpClient();
            this.SpeechOptions = opts?? new SpeechOptions();
        }

        public HttpClient Client { get; private set;}
        public SpeechOptions SpeechOptions { get; private set; }
        public string TextToServicePath { get; private set; }
        public string SpeechToTextPath { get; private set; }
        public ITokenAccessor TokenAccessor { get; }
        public string ResourceName { get; private set;}
        public HttpMethod Method {get; private set;} = HttpMethod.Post;

        private HttpRequestMessage BuildTextToSpeechMessage(string text,string voiceName){
            var req = new HttpRequestMessage();
            req.Method = this.Method;
            req.RequestUri = new Uri(this.TextToServicePath);
            req.Headers.Add("Authorization", $"Bearer {this.TokenAccessor.Token}");
            // required !
            req.Headers.Add("Connection", "Keep-Alive");
            // required !
            req.Headers.Add("User-Agent", this.ResourceName);
            req.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
            var body = $"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'><voice name='{voiceName}'>{text}</voice></speak>";
            req.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/ssml+xml");
            return req;
        }



        public async Task<Stream> FetchSpeechAsync(string text,string voiceName="Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)"){
            var req = this.BuildTextToSpeechMessage(text,voiceName);
            var resp = await this.Client.SendAsync(req);
            if (!resp.IsSuccessStatusCode){
                return null;
            }
            return await resp.Content.ReadAsStreamAsync();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="contentType"> "audio/wav; codecs=audio/pcm; samplerate=16000" or "audio/ogg; codecs=opus" </param>
        /// <param name="language">""</param>
        /// <returns></returns>
        private HttpRequestMessage BuildSpeechToTextMessage(Stream stream,string contentType,string language){
            var req = new HttpRequestMessage();
            req.Method = this.Method;
            req.RequestUri = new Uri($"{this.SpeechToTextPath}?language={language}");
            req.Headers.Add("Authorization", $"Bearer {this.TokenAccessor.Token}");
            req.Headers.Add("Connection", "Keep-Alive");
            req.Headers.Add("Transfer-Encoding","chunked");
            req.Headers.Add("Accept","application/json");

            var streamContent=new StreamContent(stream);
            streamContent.Headers.TryAddWithoutValidation("Content-Type",contentType);
            req.Content = streamContent;
            return req;
        }


        public async Task<Stream> FetchTextAsStreamResultAsync(Stream stream,string contentType,string language){
            var req = this.BuildSpeechToTextMessage(stream,contentType,language);
            var resp = await this.Client.SendAsync(req);
            if (!resp.IsSuccessStatusCode){
                var tips= await resp.Content.ReadAsStringAsync();
                return null;
            }
            return await resp.Content.ReadAsStreamAsync();
        }

    }
}