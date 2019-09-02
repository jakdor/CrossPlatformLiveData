using Android.Support.V7.App;

namespace CrossPlatformLiveData.Android
{
    /// <summary>
    /// AppCompatDialogFragment with CrossPlatformLiveData LifecycleManager linked to lifecycle events
    /// </summary>
    public class LiveDataAppCompatDialogFragment : AppCompatDialogFragment
    {
        protected readonly ILifecycleManager LifecycleManager = new LifecycleManager();

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (hidden)
            {
                LifecycleManager.OnPause();
            }
            else
            {
                LifecycleManager.OnResume();
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            if (!IsHidden)
            {
                LifecycleManager.OnResume();
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            if (!IsHidden)
            {
                LifecycleManager.OnPause();
            }
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            LifecycleManager.OnDestroyView();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LifecycleManager.Dispose();
        }
    }
}