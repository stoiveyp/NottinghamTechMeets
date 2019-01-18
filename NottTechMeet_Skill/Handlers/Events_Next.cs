using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;

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
            var results = await S3Helper.GetTechMeet(BucketName, id);
            var eventToRecognise = results.Events.FirstOrDefault();

            if (eventToRecognise == null)
            {
                return SpeechHelper.NoEvent();
            }

            return SpeechHelper.RespondToEvent(eventToRecognise);
        }
    }
}
