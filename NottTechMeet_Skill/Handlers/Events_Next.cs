﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.APL;
using Alexa.NET.APL.DataSources;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using Alexa.NET.Response.APL;
using Meetup.NetStandard.Response.Groups;
using Newtonsoft.Json;
using NodaTime;
using NottTechMeet_IO;

namespace NottTechMeet_Skill.Handlers
{
    public class Events_Next : IAlexaRequestHandler
    {
        public Events_Next(string bucketName)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; set; }

        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intent
                   && intent.Intent.Name == Consts.IntentNextEvent;
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            var request = information.SkillRequest as APLSkillRequest;
            var id = ((IntentRequest)request.Request).Intent.Slots[Consts.SlotEvent].Id();
            var currentDate = LocalDate.FromDateTime(DateTime.Now);

            var meetup = new TechMeetState { GroupName = id };
            var rawEvents = await meetup.GetEventsFromS3();
            var groupData = await meetup.GetGroupFromS3();

            var events = rawEvents.ToLocalEventTime();

            if (!events.Any())
            {
                return SpeechHelper.NoEvent();
            }

            var eventToRecognise =
                (events.Any(l => l.Date > currentDate)
                    ? events.Where(e => e.Date > currentDate)
                    : events).First();

            return SpeechHelper.SingleEventResponse(request, eventToRecognise, currentDate,groupData, "I've got information on a meetup event. ");
        }
    }
}
