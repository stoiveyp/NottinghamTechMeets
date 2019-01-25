using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class SessionEnded:SynchronousRequestHandler
    {
        public override bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is SessionEndedRequest;
        }

        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            return ResponseBuilder.Empty();
        }
    }
}
