using Amazon.CDK;
using Amazon.CDK.AWS.StepFunctions;

namespace HelloCdk
{
    public class MeetupDataStateMachine:StateMachine
    {
        public MeetupDataStateMachine(Construct scope,StepFunctionLambdaList list):base(scope,"notttechcronprocess",new StateMachineProps
        {
            StateMachineName = "notttechcronprocess"
        })
        {
            
        }
    }
}