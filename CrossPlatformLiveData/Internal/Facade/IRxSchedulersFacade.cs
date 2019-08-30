using System.Reactive.Concurrency;

namespace CrossPlatformLiveData.Internal.Facade
{
    internal interface IRxSchedulersFacade
    {
        IScheduler Io();
        IScheduler Computation();
        IScheduler NewThread();
        IScheduler Default();
        IScheduler Trampoline();
        IScheduler Ui();
    }
}
