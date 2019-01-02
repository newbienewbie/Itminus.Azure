using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Itminus.Azure.Abstraction{

    public class TokenAccessor: ITokenAccessor
    {

        public TokenAccessor(string fetchUri,string subscriptionKey, int refreshTokenDuration ,HttpClient httpClient=null)
        {
            this._fetchUri = fetchUri;
            this._subscriptionKey = subscriptionKey;
            this.RefreshTokenDuration = refreshTokenDuration;

            this._httpClient = httpClient ?? new HttpClient();
            this.Token = FetchToken().Result;

            // renew the token on set duration.
            this._accessTokenRenewer = new Timer(
                new TimerCallback(OnTokenExpiredCallback),
                this,
                TimeSpan.FromMilliseconds(RefreshTokenDuration),
                TimeSpan.FromMilliseconds(-1)
            );
        }

        private string _fetchUri;
        private string _subscriptionKey;
        private HttpClient _httpClient;
        private Timer _accessTokenRenewer;

        public virtual string Token {get;private set;}

        /// <summary>
        /// in `ms`
        /// </summary>
        /// <value></value>
        public virtual int RefreshTokenDuration { get; private set;} = 9*60*1000;

        protected virtual void RenewAccessToken()
        {
            this.Token = this.FetchToken().Result;
            Console.WriteLine($"new token renewed : {this.Token}");
        }

        protected virtual void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            finally
            {
                this._accessTokenRenewer.Change(
                    TimeSpan.FromMilliseconds(RefreshTokenDuration), 
                    TimeSpan.FromMilliseconds(-1)
                );
            }
        }

        public virtual async Task<string> FetchToken()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.Headers.Add("Ocp-Apim-Subscription-Key", this._subscriptionKey);
            request.RequestUri = new UriBuilder(this._fetchUri).Uri;
            var result = await this._httpClient.SendAsync(request);
            return await result.Content.ReadAsStringAsync();
        }
    }

}