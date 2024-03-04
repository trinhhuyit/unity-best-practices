using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Architecture.Model
{
    [Serializable]
    public class DailyRewardItemModel 
    {
            
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("quantity")] public int Quantity { get; set; }
    }
        
    [Serializable]
    public class DailyRewardModel 
    {
        [JsonProperty("day")] public int Day { get; set; }

        [JsonProperty("rewards")] public List<DailyRewardItemModel> Rewards;
    }
}