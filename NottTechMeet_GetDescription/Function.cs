using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Meetup.NetStandard;
using NottTechMeet_IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NottTechMeet_GetDescription
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="state"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<TechMeetState> FunctionHandler(TechMeetState state, ILambdaContext context)
        {
            var token = System.Environment.GetEnvironmentVariable("apitoken");
            var meetup = MeetupClient.WithApiToken(token);
            var group = await meetup.Groups.Get(state.GroupName,new []{"plain_text_no_images_description"});

            await state.SaveFullGroupToS3(group.Data);

            return state;
        }
    }
}
