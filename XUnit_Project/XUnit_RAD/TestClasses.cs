using RADImplementationProject;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace XUnit_RAD
{
    public class TestClasses
    {
        private readonly ITestOutputHelper output;
        public TestClasses(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestBitGenerator()
        {
            for (int i = 0; i < 10000; i++)
            {
                Assert.True(Generator.GenerateBits(3)[0] < 8);
            }

            for (int i = 0; i < 10000; i++)
            {
                byte[] bs = Generator.GenerateBits(9);
                Assert.True(bs[0] < 256);
                Assert.True(bs[1] < 2);
            }

            //Etc..
        }
    }
}
