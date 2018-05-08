using System;
using System.Threading.Tasks;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Scheduling;

namespace Dmarc.MxSecurityTester.MxTester
{
    internal interface IMxSecurityTesterProcessorRunner
    {
        Task Run();
    }

    internal class MxSecurityTesterProcessorRunner : IMxSecurityTesterProcessorRunner
    {
        private readonly IScheduler _scheduler;
        private readonly IMxSecurityTesterProcessor _mxSecurityTesterProcessor;
        private readonly IMxSecurityTesterConfig _mxSecurityTesterConfig;

        public MxSecurityTesterProcessorRunner(IScheduler scheduler,
            IMxSecurityTesterProcessor mxSecurityTesterProcessor,
            IMxSecurityTesterConfig mxSecurityTesterConfig)
        {
            _scheduler = scheduler;
            _mxSecurityTesterProcessor = mxSecurityTesterProcessor;
            _mxSecurityTesterConfig = mxSecurityTesterConfig;
        }

        public async Task Run()
        {
            await _scheduler.Start(() => _mxSecurityTesterProcessor.Process(),
                _mxSecurityTesterConfig.SchedulerRunIntervalSeconds * 1000);
        }
    }
}
