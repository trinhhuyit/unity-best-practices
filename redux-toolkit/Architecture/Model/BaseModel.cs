using System;
using Newtonsoft.Json;

namespace Architecture.Model
{
    [Serializable]
    public class BaseModel
    {
        [JsonProperty("_id")] public string Id { get; set; }
    }
}