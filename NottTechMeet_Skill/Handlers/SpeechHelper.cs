using System;
using System.Linq;
using Alexa.NET;
using Alexa.NET.Response;
using Meetup.NetStandard.Response.Events;

namespace NottTechMeet_Skill.Handlers
{
    public class SpeechHelper
    {
        public static SkillResponse NoEvent()
        {
            return ResponseBuilder.Tell("No events");
        }

        public static SkillResponse RespondToEvent(Event meetup)
        {
            return ResponseBuilder.Tell(meetup.Name);
        }

        public static SkillResponse RespondToEvent(Event[] meetups)
        {
            return ResponseBuilder.Tell($"{meetups.Length} events. First. {meetups.FirstOrDefault()?.Name}");
        }
    }
}