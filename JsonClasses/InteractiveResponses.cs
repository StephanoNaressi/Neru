using Newtonsoft.Json;

namespace Neru.JsonClasses
{
    public class InteractiveResponses
    {
        [JsonProperty("YUZU")]
        public List<string> Yuzu { get; set; }

        [JsonProperty("PAT")]
        public List<string> Pat { get; set; }
    }

    public class InteractiveResponsesMain
    {
        [JsonProperty("InteractiveResponses")]
        public InteractiveResponses Responses { get; set; }
    }

}
