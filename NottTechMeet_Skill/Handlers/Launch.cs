using System;
using Alexa.NET;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class Launch:LaunchSynchronousRequestHandler
    {
        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            return ResponseBuilder.Ask("Welcome to Nottingham Tech, ask about events from your favourite nottingham meetup", null);
        }
    }
}
