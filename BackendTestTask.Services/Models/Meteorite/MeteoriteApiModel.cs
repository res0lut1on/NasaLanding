using BackendTestTask.Services.Services.Generic.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BackendTestTask.Services.Models.Meteorite
{
    using System.Text.Json.Serialization;

    public class MeteoriteApiModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("recclass")]
        public string Class { get; set; } = default!;

        [JsonPropertyName("mass")]
        public string Mass { get; set; } = default!;

        [JsonPropertyName("fall")]
        public string Fall { get; set; } = default!;

        [JsonPropertyName("year")]
        public string Year { get; set; } = default!;

        [JsonPropertyName("reclat")]
        public string Reclat { get; set; } = default!;

        [JsonPropertyName("reclong")]
        public string Reclong { get; set; } = default!;

        [JsonPropertyName("geolocation")]
        public GeoLocationDto? GeoLocation { get; set; }
    }

    public class GeoLocationDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; set; } = new();
    }

}
