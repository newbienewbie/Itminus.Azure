

using System;
using Newtonsoft.Json;

namespace Itminus.Azure.QnA
{
    [Serializable]
    public class Metadata
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
