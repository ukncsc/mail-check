using System;
using System.Threading.Tasks;
using Dmarc.Common.Tasks;
using NUnit.Framework;

namespace Dmarc.Common.Test.Tasks
{
    [TestFixture]
    public class TaskExtensionMethodsTests
    {
        [Test]
        public async Task TaskCompletesBeforeTimeoutTaskReturned()
        {
            bool expected = true;
            Task<bool> task = Task.FromResult(expected);
            bool actual = await task.TimeoutAfter(TimeSpan.FromTicks(1));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TimeoutCompleteBeforeTaskThrowsTimeoutException()
        {
            Task<bool> task = new TaskCompletionSource<bool>().Task;
            Assert.ThrowsAsync<TimeoutException>(async () => await task.TimeoutAfter(TimeSpan.FromTicks(1)));
        }
    }
}
