using System;
using System.Xml.Serialization;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;
using Amazon.Runtime;

namespace NottTechMeet_Skill.Handlers
{
    public class Fallback:IntentNameSynchronousRequestHandler
    {
        private Random _rnd = new Random(DateTime.Now.Millisecond);
        private string[] Choices { get; }

        public Fallback() : base(BuiltInIntent.Fallback)
        {
            Choices = new[]
            {
                "really sorry, but I didn't understand what you were asking for, could you try again?",
                "I'm afraid I wasn't able to help with that request, would you mind asking a different way, see if that helps"
            };
        }
        public override SkillResponse HandleSyncRequest(AlexaRequestInformation information)
        {
            information.State.ClearSession();
            return ResponseBuilder.Ask(PickOne(),null);
        }

        private string PickOne()
        {
            return Choices[_rnd.Next(0, Choices.Length - 1)];
        }
    }
}
