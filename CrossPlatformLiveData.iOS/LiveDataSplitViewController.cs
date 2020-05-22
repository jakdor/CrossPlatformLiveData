using System;
using Foundation;
using UIKit;

namespace CrossPlatformLiveData.iOS
{
    /// <summary>
    /// UISplitViewController with CrossPlatformLiveData LifecycleManager linked to lifecycle events
    /// </summary>
    public class LiveDataSplitViewController : UISplitViewController
    {
        protected readonly ILifecycleManager LifecycleManager = new LifecycleManager();

        private NSObject _foregroundNotificationObserver;
        private NSObject _backgroundNotificationObserver;
        private bool _isCurrentlyVisible;

        public LiveDataSplitViewController()
        {
        }

        public LiveDataSplitViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            LifecycleManager.OnResume();

            _isCurrentlyVisible = true;

            if (_foregroundNotificationObserver == null)
            {
                _foregroundNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                    UIApplication.WillEnterForegroundNotification, (obj) => AppWillEnterForeground());
            }

            if (_backgroundNotificationObserver == null)
            {
                _backgroundNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(
                    UIApplication.DidEnterBackgroundNotification, (obj) => AppDidEnterBackground());
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            LifecycleManager.OnPause();

            _isCurrentlyVisible = false;

            RemoveAppNotificationObservers();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            LifecycleManager.OnDestroyView();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            RemoveAppNotificationObservers();
            LifecycleManager.Dispose();
        }

        private void RemoveAppNotificationObservers()
        {
            if (_foregroundNotificationObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_foregroundNotificationObserver);
                _foregroundNotificationObserver = null;
            }

            if (_backgroundNotificationObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_backgroundNotificationObserver);
                _backgroundNotificationObserver = null;
            }
        }

        private void AppWillEnterForeground()
        {
            if (_isCurrentlyVisible) LifecycleManager.OnResume();
        }

        private void AppDidEnterBackground()
        {
            if (_isCurrentlyVisible) LifecycleManager.OnPause();
        }
    }
}