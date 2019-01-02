using Itminus.Azure.Abstraction;
using System;
using System.Net.Http;
using System.Text;

namespace Itminus.Azure.Speech{

    public class SpeechServiceBuilder {

        public string SubScriptionKey {get;private set;}

        public HttpMethod Method {get; private set;} = HttpMethod.Post;
        public HttpClient Client { get; private set; }
        public SpeechOptions SpeechOptions { get; private set; }
        public string Region { get; private set; }

        public string SpeechServicePath {get{ return $"https://{this.Region}.tts.speech.microsoft.com/cognitiveservices/v1"; } }

        public string AccessTokenPath { get{ return $"https://{this.Region}.api.cognitive.microsoft.com/sts/v1.0/issueToken"; } }

        public SpeechServiceBuilder SetRegion(string region)
        {
            if(string.IsNullOrEmpty(region)){ throw new ArgumentNullException($"string: {nameof(region)} must not be null or empty"); }
            this.Region= region;
            return this;
        }

        public SpeechServiceBuilder SetSubScriptionKey(string key){
            if(string.IsNullOrEmpty(key)){ throw new ArgumentNullException($"string: {nameof(key)} must not be null or empty"); }
            this.SubScriptionKey = key;
            return this;
        }


        /// <summary>
        /// Nullable client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public SpeechServiceBuilder SetClient(HttpClient client){
            this.Client = client; 
            return this;
        }
        private SpeechServiceBuilder SetSpeechOptions(SpeechOptions opts)
        {
            this.SpeechOptions = opts;
            return this;
        }

        /// <summary>
        /// accept an object like { Region:"", Key:"", "Client": obj, "SpeechOptions":""}
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public SpeechServiceBuilder Configure(object o){
            Type t = o.GetType();
            var region=t.GetProperty("Region")?.GetValue(o) as string;
            var key=t.GetProperty("Key")?.GetValue(o) as string;
            var client = t.GetProperty("Client").GetValue(o) as HttpClient;
            var opts = t.GetProperty("SpeechOptions").GetValue(o) as SpeechOptions;
            return this
                .SetRegion(region)
                .SetSubScriptionKey(key)
                .SetClient(client)
                .SetSpeechOptions(opts);
        }

        public ITokenAccessor BuildSpeechTokenAccessor()
        {
            return new SpeechTokenAccessor(this.AccessTokenPath,this.SubScriptionKey,9*60*1000,this.Client);
        }


        public SpeechService Build(){
            var tokenAccessor = this.BuildSpeechTokenAccessor();
            return new SpeechService(this.SpeechServicePath,tokenAccessor,this.SpeechOptions,this.Client);
        }
    }



}
