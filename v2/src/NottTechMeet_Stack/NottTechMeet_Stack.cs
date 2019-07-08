using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SQS;

namespace HelloCdk
{
    public class NottTechMeetStack : Stack
    {
        public NottTechMeetStack(App parent, string name, IStackProps props) : base(parent, name, props)
        {

        }
    }
}
