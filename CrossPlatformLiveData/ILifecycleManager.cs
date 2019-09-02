using System;

namespace CrossPlatformLiveData
{
    /// <summary>
    /// LiveData LifecycleManager interface
    /// </summary>
    public interface ILifecycleManager
    {
        void Register<T>(ILiveData<T> liveData, Action<T> onNext, Action<Exception> onError);
        void OnResume();
        void OnPause();
        void OnDestroyView();
        void Dispose();
    }
}