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
            var skillName = Environment.GetEnvironmentVariable("skill_name") ?? "demo talk";
            return ResponseBuilder.Ask(
                $"Hi there and welcome to {skillName}. What events can we help you find?",
                new Reprompt("What events can we help you with today?"));
        }
    }
}
