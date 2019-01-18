using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using NodaTime;
using NodaTime.AmazonDate;

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
            var fromDate = currentDate;
            var toDate = dates.To;

            var id = intent.Slots[Consts.SlotEvent].Id();

            var results = await S3Helper.GetTechMeet(BucketName, id);
            var eventToRecognise = results.Events.Select(e =>
            new {
                RealDate=LocalDateTime.FromDateTime(DateTime.Parse(e.LocalDate)).Date,
                Original=e
            }).Where(d => d.RealDate >= fromDate && d.RealDate <= toDate).Select(e => e.Original).ToArray();

            if (eventToRecognise == null)
            {
                return SpeechHelper.NoEvent();
            }

            return SpeechHelper.RespondToEvent(eventToRecognise);
        }
    }
}
