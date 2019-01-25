using System;
using System.Collections.Generic;
using Alexa.NET;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;

namespace NottTechMeet_Skill.Handlers
{
    public class WhatEvent:IntentNameSynchronousRequestHandler
    {
        private static Dictionary<string, string> EventNames = new Dictionary<string, string>
        {
            {"Tech Nottingham",""},
            {"Women in Technology Nottingham",""},
            {"DotNet Notts",""},
            {"Notts JS" ,""},
            {"Nottingham IOT",""},
            {"PHP Minds",""}
        };

        public WhatEvent() : base("WhatEvent")
        {
            
        }

        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            var speech = new Speech(
                new Paragraph(new PlainText("Okay, here's a list of the meetups being monitored"))
                );

            foreach (var techEvent in EventNames)
            {
                speech.Elements.Add(
                    new Paragraph(
                        new Sentence(new Sub(techEvent.Key,techEvent.Value))
                    )
                );
            }

            return ResponseBuilder.Tell(speech);
        }
    }
}
