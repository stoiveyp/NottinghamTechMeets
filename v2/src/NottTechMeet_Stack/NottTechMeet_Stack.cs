using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SSM;
using Amazon.CDK.AWS.StepFunctions;

namespace HelloCdk
{
    public class NottTechMeetStack : Stack
    {
        public NottTechMeetStack(App parent, string name, IStackProps props) : base(parent, name, props)
        {
            var bucketProps = new BucketProps
            {
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL
            };
            var assetData = new Bucket(this, "notttech_assetdata", bucketProps);
            var eventData = new Bucket(this, "notttech_eventdata", bucketProps);

            var ssm = new StringListParameter(this, "notttech_grouplist", new StringListParameterProps
            {
                ParameterName = "notttech_grouplist",
                StringListValue = new[]
                {
                    "Tech-Nottingham",
                    "Women-In-Tech-Nottingham",
                    "Notts-Techfast",
                    "Nottingham-IoT-Meetup"
                }
            });

            var lambdas = new StepFunctionLambdaList(this);
            lambdas.SetSSMParameter(ssm);

            new StateMachine(this, "meetupdataworkflow", new StateMachineProps
            {
                Definition = MeetupDataStateMachine.GetDefinition(this,lambdas),
            });
        }
    }
}
