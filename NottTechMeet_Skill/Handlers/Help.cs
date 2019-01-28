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
            if (information.State.GetSession<SkillActivities>(SessionKeys.CurrentActivity) == SkillActivities.Event)
            {

            }

            return ResponseBuilder.Ask(@"I can tell you about several meetups in and around the Nottingham area.
If you're unsure of what meetups are available say, what meetups can I pick from,
                                       If you have a meetup you want to know about, ask when the next meetup is and include the name of the event. Once you've listened to the detail of an event you can ask to have a reminder set.
", null,new Session());
        }
    }
}
