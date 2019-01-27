using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using NodaTime;
using NodaTime.AmazonDate;
using NottTechMeet_IO;

namespace NottTechMeet_Skill.Handlers
{
    public class EventsAllWithDates:IAlexaRequestHandler
    {
        private string[] Meetups = new[]
        {
            "Nottingham-IoT-Meetup",
            "dotnetnotts",
            "NottsJS",
            "Women-In-Tech-Nottingham",
            "PHPMiNDS-in-Nottingham",
            "tech-nottingham"
        };

        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intent
                   && intent.Intent.Name == Consts.IntentAllEvent
                   && intent.Intent.Slots.ContainsKey(Consts.SlotDateRange)
                   && !string.IsNullOrWhiteSpace(intent.Intent.Slots[Consts.SlotDateRange].Value);
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            information.State.ClearSession();
            var intent = ((IntentRequest)information.SkillRequest.Request).Intent;

            var dates = AmazonDateParser.Parse(intent.Slots[Consts.SlotDateRange].Value);
            var currentDate = LocalDate.FromDateTime(DateTime.Now);

            var meetupTasks = Meetups.Select(m => new TechMeetState {GroupName = m}.GetEventsFromS3());
            var rawEvents = (await Task.WhenAll(meetupTasks)).SelectMany(t => t);

            var eventToRecognise = rawEvents.ToLocalEventTime()
                .Where(d => d.Date >= dates.From && d.Date <= dates.To).Where(d => d.Date >= currentDate).ToArray();

            if (!eventToRecognise.Any())
            {
                return SpeechHelper.NoEvent(true);
            }

            return SpeechHelper.RespondToEvent(eventToRecognise, currentDate);
        }
    }
}
