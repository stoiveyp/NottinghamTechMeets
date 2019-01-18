using System;
using System.Collections.Generic;
using System.Linq;
using Meetup.NetStandard.Response.Events;
using NodaTime;

namespace NottTechMeet_Skill
{
    public static class LocalEventTimeExtensions
    {
        public static IEnumerable<LocalEventTime> ToLocalEventTime(this List<Event> events)
        {
            return events.Select(e =>
                new LocalEventTime
                {
                    Event = e,
                    Date = LocalDateTime.FromDateTime(DateTime.Parse(e.LocalDate)).Date
                }).ToArray();
        }
    }
}
