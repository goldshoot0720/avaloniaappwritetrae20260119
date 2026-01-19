using System;
using System.Text.Json.Serialization;

namespace avaloniaappwritetrae20260119.Models
{
    public class Subscription
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("site")]
        public string Site { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("nextdate")]
        public DateTime NextDate { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; } = string.Empty;

        [JsonPropertyName("account")]
        public string Account { get; set; } = string.Empty;

        [JsonPropertyName("$createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("$updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
