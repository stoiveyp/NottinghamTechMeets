using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.RequestHandlers;
using Alexa.NET.Response;

namespace NottTechMeet_Skill
{
    public class StateInterceptor:IAlexaRequestInterceptor
    {
        public async Task<SkillResponse> Intercept(AlexaRequestInformation information, RequestInterceptorCall next)
        {
            var result = await next(information);
            if (result.SessionAttributes == null)
            {
                result.SessionAttributes = information.State.Session.Attributes;
            }

            return result;
        }
    }
}
