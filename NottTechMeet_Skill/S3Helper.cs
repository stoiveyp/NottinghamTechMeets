using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using NottTechMeet_IO;

namespace NottTechMeet_Skill
{
    public static class S3Helper
    {
        public static JsonSerializer Serializer = JsonSerializer.Create();

        public static async Task<int> EventCount(string bucket, string[] keys)
        {
            var tasks = keys.Select(key => EventCountSelect(bucket,key)).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Where(t => t.IsCompletedSuccessfully).Aggregate(0, (total, value) => total + value.Result);
        }

        public static async Task<int> EventCountSelect(string bucket, string key)
        {
            var state = await GetTechMeet(bucket, key);
            return state.Events.Count;
        }

        private static async Task<TechMeetState> GetTechMeet(string bucket, string key)
        {
            var s3 = new AmazonS3Client();
            var state = await s3.GetObjectAsync(bucket, key);
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
