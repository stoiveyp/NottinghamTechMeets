using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Meetup.NetStandard.Response.Events;
using Newtonsoft.Json.Linq;
using NottTechMeet_IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NottTechMeet_GetEvents
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<TechMeetState> FunctionHandler(TechMeetState input, ILambdaContext context)
        {
            var apitoken = System.Environment.GetEnvironmentVariable("apitoken");
            var meetup = Meetup.NetStandard.MeetupClient.WithApiToken(apitoken);

            var events = await meetup.Events.For(input.GroupName);

            if (input.Events == null)
            {
                input.Events = new List<Event>();
            }

            foreach (var item in events.Data)
            {
                if (input.Events.All(e => e.Id != item.Id))
                {
                    input.Events.Insert(0,item);
                }
            }

            return input;
        }
    }
}
