using System;

namespace CrossPlatformLiveData
{
    /// <summary>
    /// LiveData interface
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public interface ILiveData<T>
    {
        T Value { get; }
        void PostValue(T value);
        IDisposable Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted);
        void Observe(ILifecycleManager lifecycleManager, Action<T> onNext, Action<Exception> onError);
        void InvalidateLastPostedValue();
    }
}