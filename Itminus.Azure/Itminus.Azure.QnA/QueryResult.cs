
using Newtonsoft.Json;

namespace Itminus.Azure.QnA
{
    public class QueryResult
    {
        /// <summary>
        /// Gets or sets the list of questions indexed in the QnA Service for the given answer.
        /// </summary>
        /// <value>
        /// The list of questions indexed in the QnA Service for the given answer.
        /// </value>
        [JsonProperty("questions")]
        public string[] Questions { get; set; }

        /// <summary>
        /// Gets or sets the answer text.
        /// </summary>
        /// <value>
        /// The answer text.
        /// </value>
        [JsonProperty("answer")]
        public string Answer { get; set; }

        /// <summary>
        /// Gets or sets the answer's score, from 0.0 (least confidence) to
        /// 1.0 (greatest confidence).
        /// </summary>
        /// <value>
        /// The answer's score, from 0.0 (least confidence) to
        /// 1.0 (greatest confidence).
        /// </value>
        [JsonProperty("score")]
        public float Score { get; set; }

        /// <summary>
        /// Gets or sets metadata that is associated with the answer.
        /// </summary>
        /// <value>
        /// Metadata that is associated with the answer.
        /// </value>
        [JsonProperty(PropertyName = "metadata")]
        public Metadata[] Metadata { get; set; }

        /// <summary>
        /// Gets or sets the source from which the QnA was extracted.
        /// </summary>
        /// <value>
        /// The source from which the QnA was extracted.
        /// </value>
        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the index of the answer in the knowledge base. V3 uses
        /// 'qnaId', V4 uses 'id'.
        /// </summary>
        /// <value>
        /// The index of the answer in the knowledge base. V3 uses
        /// 'qnaId', V4 uses 'id'.
        /// </value>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }


    /// <summary>
    /// Contains answers for a user query.
    /// </summary>
    public class QueryResults
    {
        /// <summary>
        /// Gets or sets the answers for a user query,
        /// sorted in decreasing order of ranking score.
        /// </summary>
        /// <value>
        /// The answers for a user query,
        /// sorted in decreasing order of ranking score.
        /// </value>
        [JsonProperty("answers")]
        public QueryResult[] Answers { get; set; }
    }


}