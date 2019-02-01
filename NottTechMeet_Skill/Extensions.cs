using System;
using System.Collections.Generic;
using Alexa.NET.Request;
using Alexa.NET.StateManagement;
using Meetup.NetStandard.Response;
using Meetup.NetStandard.Response.Events;
using NodaTime;

namespace NottTechMeet_Skill
{
    public static class Extensions
    {
        public static void ClearSession(this ISkillState state)
        {
            if (state.Session.Attributes == null)
            {
                state.Session.Attributes = new Dictionary<string, object>();
            }
            else
            {
                state.Session.Attributes.Clear();
            }
        }

        public static LocalDateTime AsLocalDateTime(this Event meetupEvent)
        {
            return AsLocalDateTime(meetupEvent.LocalDate, meetupEvent.LocalTime);
        }

        public static LocalDateTime AsLocalDateTime(this string dateData, string timeData)
        {
            var localDate = LocalDate.FromDateTime(DateTime.Parse(dateData));
            return localDate.AsLocalDateTime(timeData);
        }

        public static LocalDateTime AsLocalDateTime(this LocalDate localDate, string timeData)
        {
            var fakeTime = DateTime.Parse($"01/01/1980 {timeData}");
            var localTime = LocalTime.FromHourMinuteSecondTick(fakeTime.Hour, fakeTime.Minute, 0, 0);
            return localDate.At(localTime);
        }
    }
}