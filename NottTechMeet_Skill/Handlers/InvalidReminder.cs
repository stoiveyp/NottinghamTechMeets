using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class InvalidReminder:IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intentRequest
                   && intentRequest.Intent.Name == Consts.IntentReminder
                   && string.IsNullOrWhiteSpace(information.State.GetSession<string>(SessionKeys.CurrentEvent));
        }

        public Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            return Task.FromResult(ResponseBuilder.Ask("You can set reminders once you've asked for an event, can I help you with anything else?",null));
        }
    }
}
