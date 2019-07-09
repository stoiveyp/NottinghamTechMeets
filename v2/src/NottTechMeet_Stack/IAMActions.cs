using System;
using System.Collections.Generic;
using System.Text;
using Amazon.CDK;
using Amazon.CDK.AWS.SSM;

namespace NottTechMeet_Stack
{
    public static class IAMActions
    {
        public const string SSMGetParameter = "ssm:GetParameter";

        public static string ParameterArn(StringListParameter parameter)
        {
            return Fn.Join("", new[]
            {
                "arn:aws:ssm:",
                Aws.REGION,
                ":",
                Aws.ACCOUNT_ID,
                ":",
                "parameter/",
                parameter.ParameterName
            });
        }
    }
}
