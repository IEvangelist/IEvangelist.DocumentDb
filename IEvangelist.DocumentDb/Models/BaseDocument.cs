using Newtonsoft.Json;
using System;

namespace IEvangelist.DocumentDb.Models
{
    public class BaseDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}