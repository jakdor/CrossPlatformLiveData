using System;

namespace CrossPlatformLiveData.Internal.Model
{
    internal class InternalObserverHolder<T> : IInternalObserverHolder
    {
        public ICustomLiveData<T> LifeData { get; set; }
        public Action<T> OnNext { get; set; }
        public Action<Exception> OnError { get; set; }
        public int Id { get; set; }
        Type IInternalObserverHolder.Type => typeof(InternalObserverHolder<T>);
    }
}
