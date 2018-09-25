using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class SessionEnd:IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation information)
        {
            if (information.SkillRequest.Request is SessionEndedRequest)
            {
                return true;
            }

            return (information.SkillRequest.Request is IntentRequest intent &&
                    intent.Intent.Name == "AMAZON.NavigateHomeIntent");
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            return ResponseBuilder.Empty();
        }
    }
}
