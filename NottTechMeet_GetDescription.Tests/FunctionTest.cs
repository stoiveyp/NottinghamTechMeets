using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using NottTechMeet_GetDescription;

namespace NottTechMeet_GetDescription.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToUpperFunction()
        {
            var descript = new Function();
            descript.FunctionHandler(null,null);
        }
    }
}
