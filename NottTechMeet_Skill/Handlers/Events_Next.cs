using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using NodaTime;

namespace NottTechMeet_Skill.Handlers
{
    public class Events_Next : IAlexaRequestHandler
    {
        public Events_Next(string bucketName)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; set; }

        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intent
                   && intent.Intent.Name == Consts.IntentNextEvent;
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            var id = ((IntentRequest) information.SkillRequest.Request).Intent.Slots[Consts.SlotEvent].Id();
            var currentDate = LocalDate.FromDateTime(DateTime.Now);

            var results = await S3Helper.GetTechMeet(BucketName, id);
            var events = results.Events.ToLocalEventTime();

            if (!events.Any())
            {
                return SpeechHelper.NoEvent();
            }

            var eventToRecognise =
                events.Any(l => l.Date > currentDate)
                    ? events.Where(e => e.Date > currentDate)
                    : events.Take(1);

            return SpeechHelper.RespondToEvent(eventToRecognise.ToArray(),currentDate);
        }
    }
}
