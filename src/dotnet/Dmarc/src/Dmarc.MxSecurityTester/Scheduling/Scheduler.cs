using System;
using System.Threading;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;

namespace Dmarc.MxSecurityTester.Scheduling
{
    public interface IScheduler
    {
        Task Start(Func<Task> job, int intervalMilliseconds);
    }

    public class Scheduler : IScheduler
    {
        private readonly ILogger _log;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public Scheduler(ILogger log)
        {
            _log = log;
        }

        public async Task Start(Func<Task> job, int intervalMilliseconds)
        {
            while (true)
            {
                RunJob(job);
                await Task.Delay(intervalMilliseconds);
            }
        }

        private void RunJob(Func<Task> job)
        {
            Task.Run(async () =>
            {
                bool available = await _semaphoreSlim.WaitAsync(0);
                if(available)
                {
                    try
                    {
                        await job();
                    }
                    catch (Exception e)
                    {
                        _log.Error($"The following error occurred: {e.Message}{Environment.NewLine}{e.StackTrace}");
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }
                }
                else
                {
                    _log.Warn("The previous task has not yet completed running");
                }
            });
        }
    }
}
