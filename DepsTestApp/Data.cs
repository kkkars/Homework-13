using System.Text.Json.Serialization;

namespace DepsTestApp
{
    class Data
    {
        [JsonPropertyName("baseAddress")]
        public string BaseAddress { get; set; }
    }
}
