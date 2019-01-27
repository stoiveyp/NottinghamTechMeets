using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
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
            information.State.ClearSession();
            var intent = ((IntentRequest) information.SkillRequest.Request).Intent;

            var dates = AmazonDateParser.Parse(intent.Slots[Consts.SlotDateRange].Value);
            var currentDate = LocalDate.FromDateTime(DateTime.Now);
            var id = intent.Slots[Consts.SlotEvent].Id();

            var meetup = new TechMeetState { GroupName = id };
            var rawEvents = await meetup.GetEventsFromS3();
            var groupData = await meetup.GetGroupFromS3();

            information.State.SetSession(SessionKeys.CurrentActivity,SkillActivities.Event);
            information.State.SetSession(SessionKeys.CurrentGroup,id);

            var eventToRecognise = rawEvents.ToLocalEventTime()
                .Where(d => d.Date >= dates.From && d.Date <= dates.To).Where(d => d.Date >= currentDate).ToArray();

            if (!eventToRecognise.Any())
            {
                return SpeechHelper.NoEvent(true);
            }

            if (eventToRecognise.Length == 1)
            {
                return SpeechHelper.SingleEventResponse((APLSkillRequest)information.SkillRequest, eventToRecognise.First(), currentDate, groupData, "I've got information on a meetup event. ");
            }

            return SpeechHelper.RespondToEvent(eventToRecognise,currentDate,groupData.Name);
        }
    }
}
