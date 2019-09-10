// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Blank
{
    [Register ("SampleViewController")]
    partial class SampleViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel clockLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton networkButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel networkLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton timerStartButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton timerStopButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (clockLabel != null) {
                clockLabel.Dispose ();
                clockLabel = null;
            }

            if (networkButton != null) {
                networkButton.Dispose ();
                networkButton = null;
            }

            if (networkLabel != null) {
                networkLabel.Dispose ();
                networkLabel = null;
            }

            if (timerStartButton != null) {
                timerStartButton.Dispose ();
                timerStartButton = null;
            }

            if (timerStopButton != null) {
                timerStopButton.Dispose ();
                timerStopButton = null;
            }
        }
    }
}