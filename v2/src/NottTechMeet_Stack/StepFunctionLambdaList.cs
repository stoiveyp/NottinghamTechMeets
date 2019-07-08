using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;

namespace HelloCdk
{
    public class StepFunctionLambdaList
    {
        public StepFunctionLambdaList(Construct scope)
        {
            var lambda = new Function(scope, "notttechgettalk", new FunctionProps
            {
                Code = Code.Asset("./lambdaoutput/gettalk.zip"),
                Runtime = Runtime.DOTNET_CORE_2_1,
                Handler = "NottTechMeet_GetTalk::NottTechMeet_GetTalk.Function::FunctionHandler"
            });
        }
    }
}