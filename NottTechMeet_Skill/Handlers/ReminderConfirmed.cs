using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Reminders;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;
using Meetup.NetStandard.Response.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            var meetupDate = LocalDate.FromDateTime(DateTime.Parse(meetupEvent.LocalDate));
            var reminderDateTime = CalculateReminderDate(meetupDate,intent.Slots[Consts.SlotRelativeDate].Id()).AsLocalDateTime(intent.Slots[Consts.SlotTimeOfDay].Value);
            return await TrySendReminder(meetupEvent, information.SkillRequest, reminderDateTime);
        }

        private async Task<SkillResponse> TrySendReminder(Event meetupEvent, SkillRequest request, LocalDateTime reminderDateTime)
        {
            var speech = new Speech(
                new Paragraph(
                new Sentence(
                    new PlainText($"You have a {meetupEvent.Group.Name} meetup happening on "),
                    new SayAs(meetupEvent.AsLocalDateTime().ToDateTimeUnspecified().ToString("f"), InterpretAs.Time)
                )));

            var reminder = new Reminder
            {
                RequestTime = DateTime.Now,
                Trigger = new AbsoluteTrigger(reminderDateTime.ToDateTimeUnspecified()),
                PushNotification = PushNotification.Disabled,
                AlertInformation = new AlertInformation(new[]
                {
                    new SpokenContent
                    {
                        Locale = "en-GB",
                        Text=$"You have a {meetupEvent.Group.Name} meetup happening on {meetupEvent.AsLocalDateTime().ToDateTimeUnspecified():f} ",
                        Ssml = speech.ToXml()
                    }
                })
            };

            var client = new RemindersClient(request);

            try
            {
                await client.Create(reminder);
                return ResponseBuilder.Tell("Okay, I've created that reminder for you");
            }
            catch (InvalidOperationException ex)
            {
                var errorBody = ex.Message.Substring(ex.Message.IndexOf("body: ") + 6);
                Console.WriteLine(errorBody);
                var code = JObject.Parse(errorBody).Value<string>("code");

                switch (code)
                {
                    case "UNAUTHORIZED":
                    case "INVALID_BEARER_TOKEN":
                        return ResponseBuilder.TellWithAskForPermissionConsentCard(
                            "Unfortunately reminder permissions haven't been enabled for this skill. You can give permission by accessing the skill settings in your alexa app",
                            new[] {"alexa::alerts:reminders:skill:readwrite"});
                    case "DEVICE_NOT_SUPPORTED":
                        return ResponseBuilder.Tell("I'm afraid this device doesn't support reminders");
                    case "INVALID_ALERT_INFO":
                        Console.WriteLine(JsonConvert.SerializeObject(reminder));
                        return ResponseBuilder.Tell(
                            "Sorry, I was unable to create your reminder for an unexpected reason. Please try again");
                    default:
                    {
                        Console.WriteLine(ex.ToString());
                        return ResponseBuilder.Tell(
                            "Sorry, I was unable to create your reminder for an unexpected reason. Please try again");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return ResponseBuilder.Tell(
                    "Sorry, I was unable to create your reminder for an unexpected reason. Please try again");
            }
        }

        private LocalDate CalculateReminderDate(LocalDate fromDateTime, string dateId)
        {
            if (int.TryParse(dateId, out int relativeDays))
            {
                return fromDateTime.Plus(Period.FromDays(relativeDays));
            }

            if (Enum.TryParse(typeof(IsoDayOfWeek), dateId, true, out object dayOfWeek))
            {
                return fromDateTime.PlusDays(1).Previous((IsoDayOfWeek)dayOfWeek);
            }

            throw new InvalidOperationException("Something went badly wrong with the relative dates on the reminder");
        }
    }
}
