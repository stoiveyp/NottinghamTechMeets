using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using NodaTime;
using NottTechMeet_IO;

namespace NottTechMeet_Skill.Handlers
{
    class ReminderConfirmed : IAlexaRequestHandler
    {
        public bool CanHandle(AlexaRequestInformation information)
        {
            return information.SkillRequest.Request is IntentRequest intentRequest
                   && intentRequest.Intent.Name == Consts.IntentReminder
                   && !string.IsNullOrWhiteSpace(information.State.GetSession<string>(SessionKeys.CurrentEvent))
                   && intentRequest.Intent.Slots["confirmation"].ConfirmationStatus == ConfirmationStatus.Confirmed;
        }

        public async Task<SkillResponse> Handle(AlexaRequestInformation information)
        {
            var intent = ((IntentRequest)information.SkillRequest.Request).Intent;

            var events = await new TechMeetState { GroupName = information.State.GetSession<string>(SessionKeys.CurrentGroup) }.GetEventsFromS3();
            var meetupEvent = events.FirstOrDefault(e => e.Id == information.State.GetSession<string>(SessionKeys.CurrentEvent));

            if (meetupEvent == null)
            {
                return ResponseBuilder.Tell(
                    "I'm sorry, but I can't seem to find the last event you looked for. If you look at another event I can set a reminder then");
            }

            var date = CalculateReminderDate(LocalDate.FromDateTime(DateTime.Parse(meetupEvent.LocalDate)),
                intent.Slots[Consts.SlotRelativeDate].Id());
            return ResponseBuilder.Tell("And here's where I'd set the reminder, boom!");
        }

        private LocalDate CalculateReminderDate(LocalDate fromDateTime, string dateId)
        {
            if (int.TryParse(dateId, out int relativeDays))
            {
                return fromDateTime.Minus(Period.FromDays(relativeDays));
            }

            if (Enum.TryParse(typeof(IsoDayOfWeek), dateId, true, out object dayOfWeek))
            {
                return fromDateTime.Previous((IsoDayOfWeek)dayOfWeek);
            }

            throw new InvalidOperationException("Something went badly wrong with the relative dates on the reminder");
        }
    }
}
