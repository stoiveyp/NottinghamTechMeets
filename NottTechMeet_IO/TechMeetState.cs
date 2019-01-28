using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Meetup.NetStandard.Response.Events;
using Meetup.NetStandard.Response.Groups;
using Newtonsoft.Json;

namespace NottTechMeet_IO
{
    public class TechMeetState
    {
        public TechMeetState() { }

        public TechMeetState(string id)
        {
            GroupName = id;
        }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("last_updated")]
        public DateTime? LastUpdated { get; set; }

        [JsonProperty("wait")] public int Wait { get; set; } = 14400;

        [JsonIgnore] public string EnvSafeName => GroupName.Replace("-", "_");
        [JsonIgnore] public string EnvSafeEventName => EnvSafeName + "_events";
        [JsonIgnore] public string EnvSafeFullGroupName => EnvSafeName + "_group";

        public async Task SaveFullGroupToS3(Group groupData)
        {
            var s3 = new AmazonS3Client();
            var request = new PutObjectRequest
            {
                BucketName = System.Environment.GetEnvironmentVariable("bucket"),
                Key = EnvSafeFullGroupName,
                ContentBody = JsonConvert.SerializeObject(groupData)
            };
            var response = await s3.PutObjectAsync(request);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                LastUpdated = DateTime.UtcNow;
            }
        }

        public async Task SaveEventsToS3(List<Event> groupData)
        {
            var s3 = new AmazonS3Client();
            var request = new PutObjectRequest
            {
                BucketName = System.Environment.GetEnvironmentVariable("bucket"),
                Key = EnvSafeEventName,
                ContentBody = JsonConvert.SerializeObject(groupData)
            };
            var response = await s3.PutObjectAsync(request);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                LastUpdated = DateTime.UtcNow;
            }
        }

        private static JsonSerializer Serializer = JsonSerializer.Create();

        public async Task<List<Event>> GetEventsFromS3()
        {
            var s3 = new AmazonS3Client();
            var request = new GetObjectRequest
            {
                BucketName = System.Environment.GetEnvironmentVariable("bucket"),
                Key = EnvSafeEventName
            };
            try
            {
                var response = await s3.GetObjectAsync(request);
                using (var reader = new JsonTextReader(new StreamReader(response.ResponseStream)))
                {
                    return Serializer.Deserialize<List<Event>>(reader);
                }
            }
            catch (AmazonS3Exception)
            {
                return new List<Event>();
            }
        }

        public async Task<Group> GetGroupFromS3()
        {
            var s3 = new AmazonS3Client();
            var request = new GetObjectRequest
            {
                BucketName = System.Environment.GetEnvironmentVariable("bucket"),
                Key = EnvSafeFullGroupName
            };
            try
            {
                var response = await s3.GetObjectAsync(request);
                using (var reader = new JsonTextReader(new StreamReader(response.ResponseStream)))
                {
                    return Serializer.Deserialize<Group>(reader);
                }
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }
    }
}
