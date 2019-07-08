using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloCdk
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new App(new AppProps());

            // A CDK app can contain multiple stacks. You can view a list of all the stacks in your
            // app by typing `cdk list`.
            new NottTechMeetStack(app, "NottTechMeetV2", new StackProps());

            app.Synth();
        }
    }
}
