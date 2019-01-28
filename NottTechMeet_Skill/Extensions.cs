using System.Collections.Generic;
using Alexa.NET.Request;
using Alexa.NET.StateManagement;

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
    }
}