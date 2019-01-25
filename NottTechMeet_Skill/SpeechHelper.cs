using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET.Response.APL;
using Alexa.NET.Response.Ssml;
using Meetup.NetStandard.Response.Groups;
using Newtonsoft.Json;
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

        public static string PickFrom(params string[] phrases)
        {
            return phrases[new Random().Next(0, phrases.Length - 1)];
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

        public static SkillResponse SingleEventResponse(APLSkillRequest request, LocalEventTime eventToRecognise, LocalDate currentDate, Group groupData, string intro)
        {
            var response = SpeechHelper.RespondToEvent(eventToRecognise, currentDate, intro);

            if (request.Context.System.Device.IsInterfaceSupported(Consts.APLInterface))
            {
                var dateDisplay =
                    $"{eventToRecognise.Date.ToDateTimeUnspecified():MMMM dd yyyy}, {eventToRecognise.Event.LocalTime}";
                var dataSource = new KeyValueDataSource
                {
                    Properties = new Dictionary<string, object>
                    {
                        {"backgroundUrl", groupData.KeyPhoto?.HighRes ?? groupData.GroupPhoto?.HighRes},
                        {"groupName", groupData.Name},
                        {"eventDate", dateDisplay},
                        {"eventTitle", eventToRecognise.Event.Name}
                    }
                };
                var document = JsonConvert.DeserializeObject<APLDocument>(File.ReadAllText("NextEvent.json"));
                response.Response.Directives.Add(new RenderDocumentDirective
                {
                    DataSources = new Dictionary<string, APLDataSource> { { "eventData", dataSource } },
                    Document = document,
                    Token = eventToRecognise.Event.Id
                });
            }

            return response;
        }
    }
}