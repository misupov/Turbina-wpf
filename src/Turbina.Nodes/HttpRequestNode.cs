using System;
using System.Net.Http;

namespace Turbina.Nodes
{
    public class HttpRequestNode : Node
    {
        [Input]
        public Uri Uri { get; set; }

        [Output]
        public string Result { get; private set; }

        protected override void Process(ProcessingContext context)
        {
            using (var httpClient = new HttpClient())
            {
                Result = httpClient.GetStringAsync(Uri).Result;
            }
        }
    }
}