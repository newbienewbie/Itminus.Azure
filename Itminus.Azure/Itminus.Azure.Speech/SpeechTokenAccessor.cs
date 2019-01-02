using System.Net.Http;
using Itminus.Azure.Abstraction;

namespace Itminus.Azure{

    public class SpeechTokenAccessor : TokenAccessor
    {
        public SpeechTokenAccessor(string fetchUri, string subscriptionKey, int refreshTokenDuration, HttpClient httpClient = null) 
            : base(fetchUri, subscriptionKey, refreshTokenDuration, httpClient)
        {
        }
    }

}