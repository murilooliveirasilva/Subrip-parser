using Newtonsoft.Json;
using System;

namespace MM.Data
{
    public class ArquivosSubtituloDTO
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("arquivo")]
        public string arquivo { get; set; }
    }
}
