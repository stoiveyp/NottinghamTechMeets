using System.Linq;
using Alexa.NET;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;
using NodaTime;

namespace NottTechMeet_Skill
{
    public class SpeechHelper
    {
        public static SkillResponse NoEvent(bool dateRange = false)
        {
            var rangeText = dateRange ? "at that time" : "at the moment";
            return ResponseBuilder.Tell("I'm afraid I've no events for the meetup "+ rangeText +". If it's just been announced we may take a little while to update.");
        }

        public static SkillResponse RespondToEvent(LocalEventTime meetup, LocalDate currentDate, string intro)
        {
            var pronoun = meetup.Date < currentDate ? "it was" : "it's";
            var humanDate = Humanizer.DateHumanizeExtensions.Humanize(
                meetup.Date.ToDateTimeUnspecified().ToUniversalTime(),
                currentDate.ToDateTimeUnspecified().ToUniversalTime());

            var starter = new Sentence();
            starter.Elements.Add(new PlainText($"{intro}, {pronoun} {humanDate}, {meetup.Date.ToDateTimeUnspecified().ToString("dddd dd MMM")}, and is titled {meetup.Event.Name}"));


            var speech = new Speech(starter);
            speech.Elements.Add(new Break{Strength = BreakStrength.Medium});
            speech.Elements.Add(new Paragraph(new Sentence("Is there another meetup I could help with?")));
            return ResponseBuilder.Ask(speech,null);
        }

        public static SkillResponse RespondToEvent(LocalEventTime[] meetups, LocalDate currentDate)
        {
            var inThePast = meetups.Where(e => e.Date < currentDate);
            var inTheFuture = meetups.Where(e => e.Date > currentDate);
            var todayEvent = meetups.Where(e => e.Date == currentDate);

            var speech = new Speech();

            if (inThePast.Any())
            {
                var previousEvent = new Paragraph();
                speech.Elements.Add(previousEvent);
            }

            if (inTheFuture.Any())
            {
                var futureEvents = new Paragraph();
                speech.Elements.Add(futureEvents);
            }

            return ResponseBuilder.Tell("whatever");
        }
    }
}