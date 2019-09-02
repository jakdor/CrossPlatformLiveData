using System.Reactive.Concurrency;

namespace CrossPlatformLiveData
{
    /// <summary>
    /// Facade for changing schedulers during testing or if other types of schedulers are required
    /// Naming scheme consistent with global ReactiveX conventions
    /// </summary>
    public interface IRxSchedulersFacade
    {
        /// <summary>
        /// Passed to all SubscribeOn(...)
        /// </summary>
        /// <returns>IScheduler</returns>
        IScheduler Io();

        /// <summary>
        /// Passed to all ObserveOn(...)
        /// </summary>
        /// <returns>IScheduler</returns>
        IScheduler Ui();
    }
}
