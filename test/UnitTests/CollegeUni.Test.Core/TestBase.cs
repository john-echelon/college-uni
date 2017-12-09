using System;
using Xunit;

namespace CollegeUni.Api.Test.Core
{
    public abstract class TestBase : IDisposable
    {
        protected TestBase()
        {
            TestSetup();
        }

        public void Dispose()
        {
            TestCleanup();
        }

        protected virtual void TestSetup()
        {
        }

        protected virtual void TestCleanup()
        {
        }
    }
}
