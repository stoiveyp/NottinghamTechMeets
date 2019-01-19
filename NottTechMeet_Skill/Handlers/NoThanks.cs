using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class NoThanks : IntentNameSynchronousRequestHandler
    {
        public NoThanks() : base(BuiltInIntent.No) { }


        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            return ResponseBuilder.Tell("Okay, no problem");
        }
    }
}
