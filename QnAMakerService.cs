using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Itminus.Azure.QnA
{
    public class QnAMakerService{

        internal QnAMakerService(string host,string path,string endpointKey, QnAMakerOptions opts ,HttpClient client){
            this.Host=host;
            this.Path = path;
            this.EndpointKey = endpointKey;
            this.Client = client?? new HttpClient();
            this.QnAMakerOptions = opts?? new QnAMakerOptions();
        }

        public string Host { get; private set;}
        public string EndpointKey {get;private set;}
        public HttpClient Client { get; private set;}
        public QnAMakerOptions QnAMakerOptions { get; private set; }
        public string Path {get; private set;}
        public HttpMethod Method {get; private set;} = HttpMethod.Post;

        private HttpRequestMessage BuildGetAnswerRequestMessage(string question, QnAMakerOptions opts){
            if(opts==null){ throw new ArgumentNullException($"{nameof(opts)} must not be null!");}
            this.ValidateOptions(opts);
            var req = new HttpRequestMessage();
            req.Method = this.Method;
            req.RequestUri = new Uri(this.Host+ this.Path);
            req.Headers.Add("Authorization", "EndpointKey " + this.EndpointKey);
            // set question
            var jsonRequest = JsonConvert.SerializeObject(
                new {
                    question = question,
                    top = opts.Top,
                    strictFilters = opts.StrictFilters,
                    metadataBoost = opts.MetadataBoost,
                }, 
                Formatting.None
            );
            req.Content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
            return req;
        }

        public async Task<QueryResult[]> GetAnswer(string question, QnAMakerOptions opts=null){
            var qnaOpts= opts??this.QnAMakerOptions;
            var req = this.BuildGetAnswerRequestMessage(question,qnaOpts);
            var resp = await this.Client.SendAsync(req);
            if (!resp.IsSuccessStatusCode){
                return null;
            }
            var jsonResp = await resp.Content.ReadAsStringAsync();
            var results = JsonConvert.DeserializeObject<QueryResults>(jsonResp);
            return results.Answers.Where(a => a.Score > qnaOpts.ScoreThreshold).ToArray();
        }


        private void ValidateOptions(QnAMakerOptions options)
        {
            if (options.ScoreThreshold == 0){
                options.ScoreThreshold = 0.3F;
            }
            if (options.Top == 0) { 
                options.Top = 1;
            }
            if (options.ScoreThreshold < 0 || options.ScoreThreshold > 1){
                throw new ArgumentOutOfRangeException(nameof(options.ScoreThreshold), "Score threshold should be a value between 0 and 1");
            }
            if (options.Top < 1) {
                throw new ArgumentOutOfRangeException(nameof(options.Top), "Top should be an integer greater than 0");
            }
            if (options.StrictFilters == null){
                options.StrictFilters = new Metadata[] { };
            }
            if (options.MetadataBoost == null){
                options.MetadataBoost = new Metadata[] { };
            }
        }

    }
}