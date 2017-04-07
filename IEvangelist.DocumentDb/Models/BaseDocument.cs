using Newtonsoft.Json;
using System;

namespace IEvangelist.DocumentDb.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]

    public class BaseDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}