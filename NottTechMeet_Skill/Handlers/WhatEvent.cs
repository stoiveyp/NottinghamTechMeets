using System.Collections.Generic;
using Alexa.NET;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;

namespace NottTechMeet_Skill.Handlers
{
    public class WhatEvent : IntentNameSynchronousRequestHandler
    {
        private static Dictionary<string, string> EventNames = new Dictionary<string, string>
        {
            {"Tech Nottingham",""},
            {"Women in Technology Nottingham",""},
            {"DotNet Notts","dot net knots"},
            {"Notts JS" ,"knots j s"},
            {"Nottingham IOT","nottingham i o t"},
            {"PHP Minds","p h p minds"}
        };

        public WhatEvent() : base(Consts.IntentWhatEvent)
        {

        }

        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            var speech = new Speech(
                new Paragraph(new PlainText("Okay, here's a list of the meetups being monitored"))
                );

            foreach (var techEvent in EventNames)
            {
                var eventSentence = new Sentence();
                if (string.IsNullOrWhiteSpace(techEvent.Value))
                {
                    eventSentence.Elements.Add(new PlainText(techEvent.Key));
                }
                else
                {
                    eventSentence.Elements.Add(new Sub(techEvent.Key, techEvent.Value));
                }

                speech.Elements.Add(
                    new Paragraph(eventSentence)
                );
            }

            speech.Elements.Add(
                new Paragraph(new PlainText("to find out about a meetup say, give me details on, followed by the meetup name."))
            );

            speech.Elements.Add(
                new Paragraph(new PlainText("to find out about a meetup say, when's the meetup for, and then the meetup name"))
            );

            information.State.SetSession(SessionKeys.CurrentActivity, SkillActivities.WhatEvent);
            return ResponseBuilder.Ask(speech, null);
        }
    }
}
