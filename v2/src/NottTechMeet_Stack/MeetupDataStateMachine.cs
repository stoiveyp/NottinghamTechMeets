using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;

namespace HelloCdk
{
    public static class MeetupDataStateMachine
    {

        public static IIChainable GetDefinition(Construct scope, StepFunctionLambdaList lambdas)
        {
            var getgroups = new Task(scope, "getgroupstask", new TaskProps
            {
                Task = new RunLambdaTask(lambdas.GetTalk, new RunLambdaTaskProps())
            });

            var datagather = new Parallel(scope,"datagather",new ParallelProps())
                .Branch(new Task(scope, "getdatatask", new TaskProps
            {
                Task = new RunLambdaTask(lambdas.GetData, new RunLambdaTaskProps())
            })).Branch(new Task(scope, "geteventtask", new TaskProps
            {
                Task = new RunLambdaTask(lambdas.GetEvents, new RunLambdaTaskProps())
            }));

            var choice = new Choice(scope, "killswitch", new ChoiceProps())
                .When(Condition.BooleanEquals("$.kill", true), new Succeed(scope, "finished", new SucceedProps()))
                .When(Condition.BooleanEquals($"$.kill", false), getgroups);

            return getgroups.Next(datagather).Next(choice);
        }
    }
}