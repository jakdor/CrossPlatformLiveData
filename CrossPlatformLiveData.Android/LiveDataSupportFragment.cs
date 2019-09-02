﻿using Android.Support.V4.App;

namespace CrossPlatformLiveData.Android
{
    /// <summary>
    /// Support Fragment with CrossPlatformLiveData LifecycleManager linked to lifecycle events
    /// </summary>
    public class LiveDataSupportFragment : Fragment
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