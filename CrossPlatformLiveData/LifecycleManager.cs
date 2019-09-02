using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using CrossPlatformLiveData.Internal.Model;

namespace CrossPlatformLiveData
{
    /// <inheritdoc />
    /// <summary>
    /// Provides automatic lifecycle awareness for LiveData
    /// </summary>
    public class LifecycleManager : ILifecycleManager
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly IList<IInternalObserverHolder> _subscriptions = new List<IInternalObserverHolder>();
        private int _internalIdSequence;
  
        /// <summary>
        /// Registers new subscription and save observer info internally for resubscribing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liveData"></param>
        /// <param name="onNext"></param>
        /// <param name="onError"></param>
        public void Register<T>(ILiveData<T> liveData, Action<T> onNext, Action<Exception> onError)
        {
            _subscriptions.Add(new InternalObserverHolder<T>
            {
                LifeData = liveData,
                OnNext = onNext,
                OnError = onError,
                Id = _internalIdSequence++
            });
        }

        /// <summary>
        /// Call on view visible
        /// </summary>
        public void OnResume()
        {
            SubscribeAll();
        }

        /// <summary>
        /// Call on view not visible
        /// </summary>
        public void OnPause()
        {
            UnSubscribeAll();
        }

        /// <summary>
        /// Subscribe without saving observer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="liveData"></param>
        /// <param name="onNext"></param>
        /// <param name="onError"></param>
        /// <param name="internalId">Internal observable holder id</param>
        protected void ReAdd<T>(ILiveData<T> liveData, Action<T> onNext, Action<Exception> onError, int internalId)
        {
            _disposable.Add(liveData.Subscribe(onNext, onError, () =>
            {
                var sub = _subscriptions.FirstOrDefault(holder => holder.Id == internalId);
                if (sub != null) _subscriptions.Remove(sub);
            }));
        }

        /// <summary>
        /// Resubscribe all saved observers
        /// </summary>
        protected void SubscribeAll()
        {
            foreach (var sub in _subscriptions)
            {
                var typedSub = DynamicCast(sub, sub.Type);
                ReAdd(typedSub.LifeData, typedSub.OnNext, typedSub.OnError, typedSub.Id);
            }
        }

        /// <summary>
        /// Unsubscribe all
        /// </summary>
        protected void UnSubscribeAll()
        {
            _disposable.Clear();
        }

        /// <summary>
        /// Resets LifecycleManager, can be reused 
        /// </summary>
        public void OnDestroyView()
        {
            _subscriptions.Clear();
            _disposable.Clear();
        }

        /// <summary>
        /// Destroy CompositeDisposable and clear saved observers list
        /// </summary>
        public void Dispose()
        {
            if (_subscriptions.Any()) _subscriptions.Clear();
            if (!_disposable.IsDisposed) _disposable.Dispose();
        }

        private static dynamic DynamicCast(dynamic obj, Type castTo) => Convert.ChangeType(obj, castTo);
    }
}
