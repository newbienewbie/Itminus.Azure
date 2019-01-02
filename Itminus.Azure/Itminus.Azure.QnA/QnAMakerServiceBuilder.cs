using System;
using System.Net.Http;
using System.Text;

namespace Itminus.Azure.QnA{
    public class QnAMakerServiceBuilder {

        public string Host { get; private set;}
        public string EndpointKey {get;private set;}
        public string Path {get; private set;}

        public HttpMethod Method {get; private set;} = HttpMethod.Post;
        public HttpClient Client { get; private set; }
        public QnAMakerOptions QnAMakerOptions { get; private set; }

        public QnAMakerServiceBuilder SetHost(string host)
        {
            if(string.IsNullOrEmpty(host)){ throw new ArgumentNullException($"string: {nameof(host)} must not be null or empty"); }
            this.Host = host;
            return this;
        }

        public QnAMakerServiceBuilder SetEndPointKey(string key){
            if(string.IsNullOrEmpty(key)){ throw new ArgumentNullException($"string: {nameof(key)} must not be null or empty"); }
            this.EndpointKey = key;
            return this;
        }

        public QnAMakerServiceBuilder SetPath(string path){
            if(string.IsNullOrEmpty(path)){ throw new ArgumentNullException($"string: {nameof(path)} must not be null or empty"); }
            this.Path = path;
            return this;
        }

        /// <summary>
        /// Nullable client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public QnAMakerServiceBuilder SetClient(HttpClient client){
            this.Client = client; 
            return this;
        }
        private QnAMakerServiceBuilder SetQnAMakerOptions(QnAMakerOptions opts)
        {
            this.QnAMakerOptions = opts;
            return this;
        }

        public QnAMakerServiceBuilder Configure(object o){
            Type t = o.GetType();
            var host=t.GetProperty("Host")?.GetValue(o) as string;
            var ep=t.GetProperty("Key")?.GetValue(o) as string;
            var path= t.GetProperty("Path").GetValue(o) as string;
            var client = t.GetProperty("Client").GetValue(o) as HttpClient;
            var opts = t.GetProperty("QnAMakerOptions").GetValue(o) as QnAMakerOptions;
            return this
                .SetHost(host).SetPath(path)
                .SetEndPointKey(ep)
                .SetClient(client)
                .SetQnAMakerOptions(opts);
        }


        public QnAMakerService Build(){
            return new QnAMakerService(this.Host,this.Path,this.EndpointKey,this.QnAMakerOptions,this.Client);
        }
    }



}
