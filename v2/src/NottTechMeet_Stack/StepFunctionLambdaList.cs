using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SSM;
using NottTechMeet_Stack;

namespace HelloCdk
{
    public class StepFunctionLambdaList
    {
        public Function GetTalk { get; }
        public Function GetEvents { get; }

        public Function GetData { get; }

        public StepFunctionLambdaList(Construct scope)
        {
            GetTalk = new Function(scope, "notttechgettalk", new FunctionProps
            {
                Code = Code.Asset("./lambdaoutput/gettalk.zip"),
                Runtime = Runtime.DOTNET_CORE_2_1,
                Handler = "NottTechMeet_GetTalk::NottTechMeet_GetTalk.Function::FunctionHandler",
                Timeout = Duration.Seconds(30)
            });

            GetEvents = new Function(scope, "notttechgetevent", new FunctionProps
            {
                Code = Code.Asset("./lambdaoutput/getevents.zip"),
                Runtime = Runtime.DOTNET_CORE_2_1,
                Handler = "NottTechMeet_PullMeetupEvents::NottTechMeet_PullMeetupEvents.Function::FunctionHandler",
                Timeout = Duration.Seconds(30)
            });

            GetData = new Function(scope, "notttechgetdata", new FunctionProps
            {
                Code = Code.Asset("./lambdaoutput/getdata.zip"),
                Runtime = Runtime.DOTNET_CORE_2_1,
                Handler = "NottTechMeet_PullMeetupData::NottTechMeet_PullMeetupData.Function::FunctionHandler",
                Timeout = Duration.Seconds(30)
            });
        }

        public void SetSSMParameter(StringListParameter parameter)
        {
            GetTalk.AddEnvironment("parameter", parameter.ParameterName);
            GetTalk.Role.AddToPolicy(new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] {IAMActions.SSMGetParameter},
                Resources = new[] { IAMActions.ParameterArn(parameter) }
            })
            {
                Effect = Effect.ALLOW
            });
        }
    }
}