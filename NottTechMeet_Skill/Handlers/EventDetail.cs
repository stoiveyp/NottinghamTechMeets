using System;
using System.Threading.Tasks;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class EventDetail:IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation information)
        {
            return false;
        }

        public Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            throw new NotImplementedException();
        }
    }
}
