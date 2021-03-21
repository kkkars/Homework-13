using System.Text.Json.Serialization;

namespace DepsTestApp.Rates
{
    class CurrencyData
    {
        [JsonPropertyName("cc")]
        public string CC { get; set; }

        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }
    }
}
