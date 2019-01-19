using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class FinishedIntents : IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intent &&
                   (intent.Intent.Name == BuiltInIntent.No ||
                    intent.Intent.Name == BuiltInIntent.Cancel ||
                    intent.Intent.Name == BuiltInIntent.Stop);
        }

        public Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            return Task.FromResult(ResponseBuilder.Tell("Okay. Hope you enjoy the meetups"));
        }
    }
}
