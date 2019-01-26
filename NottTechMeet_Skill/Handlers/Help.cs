using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class Help:IntentNameSynchronousRequestHandler
    {
        public Help() : base(BuiltInIntent.Help)
        {
        }

        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            return ResponseBuilder.Ask(@"I can tell you about several meetups in and around the Nottingham area.
                                       To ask about a meetup, ask when the next meetup is and include the name of the event.
if you're unsure of what meetups are available say, what meetups can I pick from",null,new Session());
        }
    }
}
