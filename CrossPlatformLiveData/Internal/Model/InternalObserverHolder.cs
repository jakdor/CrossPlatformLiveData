using System;

namespace CrossPlatformLiveData.Internal.Model
{
    /// <summary>
    /// Used internally as observer storage model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class InternalObserverHolder<T> : IInternalObserverHolder
    {
        public ILiveData<T> LifeData { get; set; }
        public Action<T> OnNext { get; set; }
        public Action<Exception> OnError { get; set; }
        public int Id { get; set; }
        Type IInternalObserverHolder.Type => typeof(InternalObserverHolder<T>);
    }
}
