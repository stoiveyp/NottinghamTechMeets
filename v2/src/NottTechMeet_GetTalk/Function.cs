using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Newtonsoft.Json.Linq;
using NottTechMeet_Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NottTechMeet_GetTalk
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<TechMeetModel> FunctionHandler(JObject input, ILambdaContext context)
        {
            var ssm = new AmazonSimpleSystemsManagementClient(RegionEndpoint.EUWest1);
            var paramValue = await ssm.GetParameterAsync(new GetParameterRequest { Name = Environment.GetEnvironmentVariable("parameter") });
            return new TechMeetModel
            {
                Groups = paramValue.Parameter.Value.Split(',')
            };
        }
    }
}
