using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Reminders;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;
using Meetup.NetStandard.Response.Events;
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
            var aplRequest = (APLSkillRequest)information.SkillRequest;
            var intent = ((IntentRequest)aplRequest.Request).Intent;

            var events = await new TechMeetState { GroupName = information.State.GetSession<string>(SessionKeys.CurrentGroup) }.GetEventsFromS3();
            var meetupEvent = events.FirstOrDefault(e => e.Id == information.State.GetSession<string>(SessionKeys.CurrentEvent));

            if (meetupEvent == null)
            {
                return ResponseBuilder.Tell(
                    "I'm sorry, but I can't seem to find the last event you looked for. If you look at another event I can set a reminder then");
            }

            var date = CalculateReminderDate(LocalDate.FromDateTime(DateTime.Parse(meetupEvent.LocalDate)),
                intent.Slots[Consts.SlotRelativeDate].Id());
            var timePieces = intent.Slots[Consts.SlotTimeOfDay].Value.Split(':');
            date.At(new LocalTime(int.Parse(timePieces[0]), int.Parse(timePieces[1])));
            return await TrySendReminder(meetupEvent, date.ToDateTimeUnspecified(), aplRequest.Context.System.ApiAccessToken);
        }

        private async Task<SkillResponse> TrySendReminder(Event meetupEvent, DateTime reminderDate, string accessToken)
        {
            var speech = new Speech(
                new Paragraph(
                new Sentence(
                    new PlainText($"You have a {meetupEvent.Group.Name} meetup happening on "),
                    new SayAs(reminderDate.ToString("f"), InterpretAs.Time)
                )));

            var reminder = new Reminder
            {
                RequestTime = DateTime.Now,
                Trigger = new AbsoluteTrigger(reminderDate),
                PushNotification = PushNotification.Disabled,
                AlertInformation = new AlertInformation(new[]
                {
                    new SpokenContent
                    {
                        Locale = "en-GB",
                        Ssml = speech.ToXml()
                    }
                })
            };

            var client = new RemindersClient(RemindersClient.ReminderDomain, accessToken);

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
                        return ResponseBuilder.Tell(
                            "Unfortunately reminder permissions haven't been enabled for this skill. You can give permission by accessing the skill settings in your alexa app");

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
