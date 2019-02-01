using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.RequestHandlers.Handlers;
using Alexa.NET.Response;
using Alexa.NET.Response.APL;
using Alexa.NET.Response.Ssml;
using Meetup.NetStandard.Response.Groups;
using Newtonsoft.Json;
using NottTechMeet_IO;

namespace NottTechMeet_Skill.Handlers
{
    public class EventDetail : IntentNameRequestHandler
    {
        public EventDetail() : base(Consts.IntentEventDetail)
        {
        }

        public override async Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            var aplRequest = (APLSkillRequest)information.SkillRequest;
            var intent = (IntentRequest)information.SkillRequest.Request;
            var id = intent.Intent.Slots[Consts.SlotEvent].Id();
            var group = await new TechMeetState(id).GetGroupFromS3();

            var detail = group.ExtraFields[Consts.DataPlainTextDescription].ToString().Replace("https://", string.Empty)
                .Split("\n\n").First();
            detail = detail + (detail.Last() == '.' ? string.Empty : ".") + "\n\nYou can find out more information online";

            var response = ResponseBuilder.Ask(detail + ". Would you like to know about another meetup?",null);
            if (aplRequest.Context.System.Device.IsInterfaceSupported(Consts.APLInterface))
            {
                await AddEventDisplay(response.Response, group,detail);
            }
            information.State.ClearSession();
            information.State.SetSession(SessionKeys.CurrentActivity, SkillActivities.GroupDetail);
            information.State.SetSession(SessionKeys.CurrentGroup, id);

            return response;
        }

        private async Task AddEventDisplay(ResponseBody response, Group groupData, string initialText)
        {
            var eventData = new ObjectDataSource
            {
                Properties = new Dictionary<string, object>
                {
                    {"text",initialText}
                },
                TopLevelData = new Dictionary<string, object>
                {
                    {"backgroundUrl", groupData.GroupPhoto?.HighRes ?? groupData.KeyPhoto?.HighRes},
                    {"groupName", groupData.Name}
                },
                Transformers = new List<APLTransformer>()
            };


            var document = await S3Helper.GetDocument(System.Environment.GetEnvironmentVariable("bucket"), "assets/EventDetail.json");

            var directive = new RenderDocumentDirective
            {
                Document = document,
                DataSources = new Dictionary<string, APLDataSource>
                {
                    {"eventData", eventData}
                },
                Token = groupData.Id.ToString()
            };

            response.Directives.Add(directive);
        }

        public static void AddKaraoke(Speech speech, ObjectDataSource dataSource)
        {
            dataSource.Properties.Add("ssml", speech.ToXml());
            dataSource.Transformers.Add(APLTransformer.SsmlToText("ssml", "text"));
            dataSource.Transformers.Add(APLTransformer.SsmlToSpeech("ssml", "speech"));
        }
    }
}
