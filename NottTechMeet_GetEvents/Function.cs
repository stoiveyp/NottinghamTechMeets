using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Meetup.NetStandard;
using Meetup.NetStandard.Request.Events;
using Meetup.NetStandard.Response.Events;
using NottTechMeet_IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NottTechMeet_GetEvents
{
    public class Function
    {
        public string ApiToken { get; set; }

        public Function()
        {
        }

        public Function(string apiToken)
        {
            ApiToken = apiToken;
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<TechMeetState> FunctionHandler(TechMeetState input, ILambdaContext context)
        {
            var apitoken = ApiToken ?? Environment.GetEnvironmentVariable("apitoken");
            var meetup = MeetupClient.WithApiToken(apitoken);

            var request = new GetEventsRequest(input.GroupName)
            {
                NoEarlierThan = DateTime.UtcNow.AddMonths(-1),
                NoLaterThan = DateTime.UtcNow.AddMonths(2),
                Status = EventStatus.Past | EventStatus.Upcoming,
                Descending=true
            };

            var events = await meetup.Events.For(request);

            if (input.Events == null)
            {
                input.Events = new List<Event>();
            }

            Console.WriteLine($"found {events.Data.Length} events in response");
            var finalList = events.Data.Concat(input.Events).Distinct(new EventEquality()).OrderByDescending(e => DateTime.Parse(e.LocalDate));
            input.Events = finalList.ToList();

            return input;
        }
    }

    internal class EventEquality : IEqualityComparer<Event>
    {
        public bool Equals(Event x, Event y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Event obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
