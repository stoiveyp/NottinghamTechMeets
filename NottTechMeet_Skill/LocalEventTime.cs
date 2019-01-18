using System;
using System.Collections.Generic;
using System.Text;
using Meetup.NetStandard.Response.Events;
using NodaTime;

namespace NottTechMeet_Skill
{
    public class LocalEventTime
    {
        public LocalDate Date { get; set; }

        public Event Event { get; set; }
    }
}
