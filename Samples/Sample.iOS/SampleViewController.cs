using CrossPlatformLiveData;
using CrossPlatformLiveData.iOS;
using Sample.Core.Model;
using Sample.Core.ViewModel;
using System;
using UIKit;

namespace Blank
{
    public partial class SampleViewController : LiveDataViewController
    {
        private ISampleViewModel _viewModel;

        public SampleViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _viewModel = AppDelegate.SampleViewModel;

            timerStartButton.AddGestureRecognizer(new UITapGestureRecognizer(() => _viewModel?.StartClock()));
            timerStopButton.AddGestureRecognizer(new UITapGestureRecognizer(() => _viewModel?.StopClock()));
            networkButton.AddGestureRecognizer(new UITapGestureRecognizer(() => _viewModel?.GetFakeNetworking()));

            //Observe LiveData streams with LifecycleManager
            //Use NSLog or other error logging lib in real app
            _viewModel?.ClockLiveData.Observe(
                LifecycleManager, OnNextClockData, e => Console.WriteLine($"SampleViewController {e.ToString()}"));

            _viewModel?.FakeNetworkingLiveData.Observe(
                LifecycleManager, OnNextNetworkData, e => Console.WriteLine($"SampleViewController {e.ToString()}"));
        }

        /// <summary>
        /// Update UI with new data from ClockLiveData
        /// </summary>
        /// <param name="clockText">formatted time</param>
        private void OnNextClockData(string clockText)
        {
            clockLabel.Text = clockText;
        }

        /// <summary>
        /// Response from network fetch in RxWrapper
        /// </summary>
        /// <param name="response">Sample use of RxWrapper</param>
        private void OnNextNetworkData(RxWrapper<SampleResponse> response)
        {
            switch (response.Status)
            {
                case RxStatus.Ok:
                    networkButton.Enabled = true;
                    networkLabel.Text = $"{response.Data.Id}. - {response.Data.Name}";
                    break;
                case RxStatus.Pending:
                    networkButton.Enabled = false;
                    networkLabel.Text = "Pending request...";
                    break;
                case RxStatus.Error:
                    networkButton.Enabled = true;
                    networkLabel.Text = "Error has occured";
                    break;
            }
        }
    }
}