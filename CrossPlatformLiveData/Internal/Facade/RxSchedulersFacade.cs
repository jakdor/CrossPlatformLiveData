using System.Reactive.Concurrency;
using System.Threading;

namespace CrossPlatformLiveData.Internal.Facade
{
    /// <summary>
    /// <inheritdoc cref="IRxSchedulersFacade"/>
    /// </summary>
    internal class RxSchedulersFacade : IRxSchedulersFacade
    {
        /// <summary>
        /// Schedulers pool with smart creation and re-use
        /// <inheritdoc cref="IRxSchedulersFacade.Io"/>
        /// </summary>
        public IScheduler Io()
        {
            return TaskPoolScheduler.Default; 
        }

        /// <summary>
        /// Used for synchronizing with UI thread
        /// <inheritdoc cref="IRxSchedulersFacade.Ui"/>
        /// </summary>
        public IScheduler Ui()
        {
            return new SynchronizationContextScheduler(SynchronizationContext.Current);
        }
    }
}
