using Android.App;

namespace CrossPlatformLiveData.Android
{
    /// <summary>
    /// Activity with CrossPlatformLiveData LifecycleManager linked to lifecycle events
    /// </summary>
    public class LiveDataActivity : Activity
    {
        protected readonly ILifecycleManager LifecycleManager = new LifecycleManager();

        protected override void OnResume()
        {
            base.OnResume();
            LifecycleManager.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            LifecycleManager.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LifecycleManager.Dispose();
        }
    }
}