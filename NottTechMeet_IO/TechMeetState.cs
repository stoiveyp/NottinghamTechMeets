using System;
using System.Collections.Generic;
using System.Text;
using Meetup.NetStandard.Response.Events;
using Newtonsoft.Json;

namespace NottTechMeet_IO
{
    public class TechMeetState
    {
        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("events")]
        public List<Event> Events { get; set; }

        [JsonProperty("last_updated")]
        public DateTime? LastUpdated { get; set; }

        [JsonIgnore] public string EnvSafeGroupName => GroupName.Replace("-", "_");
    }
}
