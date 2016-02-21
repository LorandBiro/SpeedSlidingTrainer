using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpeedSlidingTrainer.CoreTests.Utils
{
    internal static class ExceptionAssert
    {
        public static void Throws(Action action, Type exceptionType)
        {
            try
            {
                action();
                Assert.Fail($"Code was expected to throw {exceptionType}, but it ran successfully.");
            }
            catch (Exception e)
            {
                if (e.GetType() != exceptionType)
                {
                    Assert.Fail($"Code was expected to throw {exceptionType}, but it threw {e.GetType()}.");
                }
            }
        }
    }
}
