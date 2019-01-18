using System.Linq;
using Alexa.NET;
using Alexa.NET.Response;
using Meetup.NetStandard.Response.Events;
using NodaTime;

namespace NottTechMeet_Skill
{
    public class SpeechHelper
    {
        public static SkillResponse NoEvent()
        {
            return ResponseBuilder.Tell("No events");
        }

        public static SkillResponse RespondToEvent(LocalEventTime meetup, LocalDate currentDate)
        {
            return ResponseBuilder.Tell(meetup.Event.Name);
        }

        public static SkillResponse RespondToEvent(LocalEventTime[] meetups, LocalDate currentDate)
        {
            return ResponseBuilder.Tell($"{meetups.Length} events. First. {meetups.FirstOrDefault()?.Event.Name}");
        }
    }
}