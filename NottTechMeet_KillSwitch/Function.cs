using System;
using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;
using NottTechMeet_IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NottTechMeet_KillSwitch
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public TechMeetState FunctionHandler(TechMeetState input, ILambdaContext context)
        {
            if (input == null)
            {
                return null;
            }

            try
            {
                var output = System.Environment.GetEnvironmentVariable(input.EnvSafeName);
                input.Active = !string.IsNullOrWhiteSpace(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unknown issue with {input.GroupName}");
                Console.WriteLine(ex.Message);
                input.Active = false;
            }

            return input;
        }
    }
}
