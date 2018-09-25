using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using NottTechMeet_IO;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NottTechMeet_UpdateS3
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
            var s3 = new AmazonS3Client();
            var request = new PutObjectRequest
            {
                BucketName = System.Environment.GetEnvironmentVariable("bucket"),
                Key = input.EnvSafeGroupName,
                ContentBody = JsonConvert.SerializeObject(input)
            };
            var response = await s3.PutObjectAsync(request);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                input.LastUpdated = DateTime.UtcNow;
            }

            return input;
        }
    }
}
