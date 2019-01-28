using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Alexa.NET.Response.APL;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using NottTechMeet_IO;

namespace NottTechMeet_Skill
{
    public static class S3Helper
    {
        public static JsonSerializer Serializer = JsonSerializer.Create();

        public static async Task<APLDocument> GetDocument(string bucket, string key)
        {
            var s3 = new AmazonS3Client();
            var state = await s3.GetObjectAsync(bucket, key);
            if (state.HttpStatusCode == HttpStatusCode.OK)
            {
                using (var reader = new JsonTextReader(new StreamReader(state.ResponseStream)))
                {
                    return Serializer.Deserialize<APLDocument>(reader);
                }
            }

            throw new InvalidOperationException("Unable to retrieve key");
        }

        public static async Task<TechMeetState> GetTechMeet(string bucket, string key)
        {
            var s3 = new AmazonS3Client();
            var state = await s3.GetObjectAsync(bucket, key.Replace("-","_"));
            if (state.HttpStatusCode == HttpStatusCode.OK)
            {
                using (var reader = new JsonTextReader(new StreamReader(state.ResponseStream)))
                {
                    return Serializer.Deserialize<TechMeetState>(reader);
                }
            }

            throw new InvalidOperationException("Unable to retrieve key");
        }
    }
}
