using Android.Support.V4.App;

namespace CrossPlatformLiveData.Android
{
    /// <summary>
    /// FragmentActivity with CrossPlatformLiveData LifecycleManager linked to lifecycle events
    /// </summary>
    public class LiveDataFragmentActivity : FragmentActivity
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

        protected override void OnStop()
        {
            base.OnStop();
            LifecycleManager.OnDestroyView();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LifecycleManager.Dispose();
        }
    }
}