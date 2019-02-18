using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using NodaTime;
using NottTechMeet_IO;

namespace NottTechMeet_Skill.Handlers
{
    public class ReminderDenied : IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intentRequest
                   && intentRequest.Intent.Name == Consts.IntentReminder
                   && !string.IsNullOrWhiteSpace(information.State.GetSession<string>(SessionKeys.CurrentEvent))
                   && intentRequest.Intent.Slots["confirmation"].ConfirmationStatus == ConfirmationStatus.Denied;
        }

        public Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            var intent = ((IntentRequest)information.SkillRequest.Request).Intent;
            intent.ConfirmationStatus = ConfirmationStatus.None;
            if (intent.Slots.ContainsKey("confirmation"))
            {
                intent.Slots["confirmation"].ConfirmationStatus = ConfirmationStatus.None;
            }
            intent.Slots[Consts.SlotRelativeDate] = new Slot { Name = Consts.SlotRelativeDate };
            intent.Slots[Consts.SlotTimeOfDay] = new Slot { Name = Consts.SlotTimeOfDay };
            return Task.FromResult(ResponseBuilder.DialogDelegate(intent));
        }
    }
}
