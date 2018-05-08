using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dmarc.Common.Tasks
{
    public static class TaskExtensionMethods
    {
        //Credited to Lawrence Johnston : http://stackoverflow.com/questions/4238345/asynchronously-wait-for-taskt-to-complete-with-timeout
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();

            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                return await task;
            }
            throw new TimeoutException("The operation has timed out.");
        }
    }
}
