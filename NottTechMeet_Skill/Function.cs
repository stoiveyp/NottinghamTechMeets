using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using NottTechMeet_Skill.Handlers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NottTechMeet_Skill
{
    public class Function
    {
        
        public AlexaRequestPipeline Pipeline { get; set; }

        public Function()
        {
            Pipeline = new AlexaRequestPipeline( new IAlexaRequestHandler[]{
                new NextEvent(),
                new TimelineEvent(),
                new GroupInfo(),
                new UnknownGroup(),
                new FallbackIntent(), 
            });
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            return Pipeline.Process(input, context);
        }
    }
}
