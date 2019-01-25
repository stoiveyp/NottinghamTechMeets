using System;
using System.Collections.Generic;
using Alexa.NET;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill.Handlers
{
    public class WhatEvent:IntentNameSynchronousRequestHandler
    {
        private static Dictionary<string, string> EventNames = new Dictionary<string, string>
        {
            {"",""}
        };

        public WhatEvent() : base("WhatEvent")
        {
            
        }

        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            return ResponseBuilder.Tell("none!");
        }
    }
}
