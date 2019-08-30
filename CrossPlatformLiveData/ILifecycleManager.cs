using System;

namespace CrossPlatformLiveData
{
    /// <summary>
    /// CustomLiveData LifecycleManager interface
    /// </summary>
    public interface ILifecycleManager
    {
        void Register<T>(ICustomLiveData<T> lifeData, Action<T> onNext, Action<Exception> onError);
        void OnResume();
        void OnPause();
        void OnDestroyView();
        void Dispose();
    }
}