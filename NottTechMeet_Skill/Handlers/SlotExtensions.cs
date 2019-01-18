using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexa.NET.Request;

namespace NottTechMeet_Skill.Handlers
{
    public static class SlotExtensions
    {
        public static bool HasId(this Slot slot)
        {
            return !string.IsNullOrWhiteSpace(slot.Id());
        }

        public static string Id(this Slot slot)
        {
            return slot.Resolution?.Authorities.FirstOrDefault()?.Values.FirstOrDefault()?.Value.Id;
        }
    }
}
