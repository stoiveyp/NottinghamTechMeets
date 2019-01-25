using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using Alexa.NET.StateManagement.S3;
using Amazon.Lambda.Core;
using Amazon.S3;
using NottTechMeet_Skill.Handlers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NottTechMeet_Skill
{
    public class Function
    {

        public AlexaRequestPipeline Pipeline { get; set; }
        public S3PersistenceStore Store { get; set; }

        public Function()
        {
            var bucketName = "notttechmeet";
            Pipeline = new AlexaRequestPipeline(new IAlexaRequestHandler[]
            {
                new Events_Calendar(bucketName), 
                new Events_Next(bucketName), 
                new EventDetail(),
                new FinishedIntents(), 
                new WhatEvent(), 
                new Help(), 
                new Launch(),
                new Fallback(),
                new SessionEnded()
            })
            { StatePersistance = Store };
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<SkillResponse> FunctionHandler(APLSkillRequest input, ILambdaContext context)
        {
            var response = await Pipeline.Process(input, context);
            return response;
        }
    }
}
