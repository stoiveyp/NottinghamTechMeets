using System;
using Newtonsoft.Json;

namespace NottTechMeet_Model
{
    public class TechMeetModel
    {
        [JsonProperty("groups")]
        public string[] Groups { get; set; }
    }
}
