using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class NextEvent : IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intentRequest && intentRequest.Intent.Name == "Meetups";
        }

        public Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            var intentRequest = information.SkillRequest.Request as IntentRequest;
            var slots = intentRequest.Intent.Slots;
            if (!slots.ContainsKey("timeslot") || string.IsNullOrEmpty(slots["timeslot"].Value))
            {
                return NextEventFunc();
            }

            return EventsInTimeline(slots["timeslot"].Value);
        }

        private Task<SkillResponse> NextEventFunc()
        {
            return Task.FromResult(ResponseBuilder.Tell("There's a talk happening right now!"));
        }

        private Task<SkillResponse> EventsInTimeline(string slot)
        {
            return Task.FromResult(ResponseBuilder.Tell("There's a talk happening right now!"));
        }
    }
}