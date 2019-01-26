using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.APL;
using Alexa.NET.APL.Commands;
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

            if (aplRequest.Context.System.Device.IsInterfaceSupported(Consts.APLInterface))
            {
                var response = ResponseBuilder.Tell(string.Empty);
                response.Response.OutputSpeech = null;
                AddEventDisplay(response.Response, group);
                return response;
            }
            information.State.ClearSession();
            information.State.SetSession(SessionKeys.CurrentActivity, SkillActivities.GroupDetail);
            information.State.SetSession(SessionKeys.CurrentGroup, id);

            return ResponseBuilder.Tell(group.ExtraFields[Consts.DataPlainTextDescription].ToString().Replace("https://",string.Empty));
        }

        private void AddEventDisplay(ResponseBody response, Group groupData)
        {
            var eventData = new ObjectDataSource
            {
                Properties = new Dictionary<string, object>(),
                TopLevelData = new Dictionary<string, object>
                {
                    {"backgroundUrl", groupData.GroupPhoto?.HighRes ?? groupData.KeyPhoto?.HighRes},
                    {"groupName", groupData.Name}
                },
                Transformers = new List<APLTransformer>()
            };

            var document = JsonConvert.DeserializeObject<APLDocument>(File.ReadAllText("EventDetail.json"));

            var directive = new RenderDocumentDirective
            {
                Document = document,
                DataSources = new Dictionary<string, APLDataSource>
                {
                    {"eventData", eventData}
                },
                Token = groupData.Id.ToString()
            };

            var speech = new Speech(groupData.ExtraFields[Consts.DataPlainTextDescription].ToString().Replace("https://",string.Empty).Split("\n\n")
                .SelectMany(t => new ISsml[] { new Paragraph(new Sentence(new PlainText(t))), new PlainText("\n\n") }).ToArray());
            AddKaraoke(speech, eventData);
            response.Directives.Add(directive);
            response.Directives.Add(new ExecuteCommandsDirective(groupData.Id.ToString(), new SpeakItem
            {
                ComponentId = "groupDescription",
                HighlightMode = "line"
            }));

        }

        public static void AddKaraoke(Speech speech, ObjectDataSource dataSource)
        {
            dataSource.Properties.Add("ssml", speech.ToXml());
            dataSource.Transformers.Add(APLTransformer.SsmlToText("ssml", "text"));
            dataSource.Transformers.Add(APLTransformer.SsmlToSpeech("ssml", "speech"));
        }
    }
}
