using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class FallbackIntent:IntentNameSynchronousRequestHandler
    {
        public FallbackIntent() : base(BuiltInIntent.Fallback) { }

        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            return ResponseBuilder.Tell(
                "Aww sorry, we're not sure how to handle that request. We hope to keep adding new features, and you can always ask for help to get more information");
        }
    }
}
