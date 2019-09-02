using Android.Support.V7.App;

namespace CrossPlatformLiveData.Android
{
    /// <summary>
    /// AppCompatActivity with CrossPlatformLiveData LifecycleManager linked to lifecycle events
    /// </summary>
    public class LiveDataAppCompatActivity : AppCompatActivity
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