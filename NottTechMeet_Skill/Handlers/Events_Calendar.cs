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
    public class Events_Calendar:IAlexaRequestHandler
    {
        private readonly string BucketName;

        public Events_Calendar(string bucketName)
        {
            BucketName = bucketName;
        }

        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intent
                   && intent.Intent.Name == Consts.IntentNextEvent
                   && intent.Intent.Slots.ContainsKey(Consts.SlotDateRange)
                   && !string.IsNullOrWhiteSpace(intent.Intent.Slots[Consts.SlotDateRange].Value);
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            var intent = ((IntentRequest) information.SkillRequest.Request).Intent;

            var dates = AmazonDateParser.Parse(intent.Slots[Consts.SlotDateRange].Value);
            var currentDate = LocalDate.FromDateTime(DateTime.Now);
            var id = intent.Slots[Consts.SlotEvent].Id();

            var meetup = new TechMeetState { GroupName = id };
            var rawEvents = await meetup.GetEventsFromS3();
            //var groupData = await meetup.GetGroupFromS3();

            var eventToRecognise = rawEvents.ToLocalEventTime()
                .Where(d => d.Date >= dates.From && d.Date <= dates.To).ToArray();

            if (!eventToRecognise.Any())
            {
                return SpeechHelper.NoEvent(true);
            }

            return SpeechHelper.RespondToEvent(eventToRecognise,currentDate);
        }
    }
}
