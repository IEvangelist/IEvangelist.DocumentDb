using Newtonsoft.Json;
using System;

namespace IEvangelist.DocumentDb.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]

    public class Person : BaseDocument
    {
        [JsonProperty]
        public string FirstName { get; set; }

        [JsonProperty]
        public string LastName { get; set; }

        [JsonProperty]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty]
        public Sex Sex { get; set; }
    }

    public enum Sex
    {
        Male, Female
    }
}
