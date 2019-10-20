using System;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using CrossPlatformLiveData;
using CrossPlatformLiveData.Android;
using Sample.Core.Model;
using Sample.Core.ViewModel;

namespace Sample.Android
{
    public class SampleFragment : LiveDataSupportFragment
    {
        private ISampleViewModel _viewModel;

        private TextView _clockTextView;
        private TextView _networkFetchTextView;
        private Button _requestNetworkFetchButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_sample, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _clockTextView = view.FindViewById<TextView>(Resource.Id.clock_label);
            _networkFetchTextView = view.FindViewById<TextView>(Resource.Id.network_label);

            _requestNetworkFetchButton = view.FindViewById<Button>(Resource.Id.network_button);
            _requestNetworkFetchButton.Click += (sender, args) => _viewModel?.GetFakeNetworking();

            view.FindViewById<Button>(Resource.Id.clock_start_button).Click +=
                (sender, args) => _viewModel?.StartClock();

            view.FindViewById<Button>(Resource.Id.clock_stop_button).Click +=
                (sender, args) => _viewModel?.StopClock();
        }

        /// <summary>
        /// Get ViewModel dependency from parent Activity, and observe LiveData streams 
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            if (Activity is ISampleFragmentContract activity)
            {
                if(_viewModel == null)
                {
                    _viewModel = activity.GetSampleViewModel();

                    //Observe LiveData streams with LifecycleManager
                    _viewModel?.ClockLiveData.Observe(
                        LifecycleManager, OnNextClockData, e => Log.Error("SampleFragment", e.ToString()));

                    _viewModel?.FakeNetworkingLiveData.Observe(
                        LifecycleManager, OnNextNetworkData, e => Log.Error("SampleFragment", e.ToString()));
                }
            }
            else
            {
                throw new Exception("Activity not implementing ISampleFragmentContract");
            }
        }

        /// <summary>
        /// Update UI with new data from ClockLiveData
        /// </summary>
        /// <param name="clockText">formatted time</param>
        private void OnNextClockData(string clockText)
        {
            _clockTextView.Text = clockText;
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
                    _requestNetworkFetchButton.Enabled = true;
                    _networkFetchTextView.Text = $"{response.Data.Id}. - {response.Data.Name}";
                    break;
                case RxStatus.Pending:
                    _requestNetworkFetchButton.Enabled = false;
                    _networkFetchTextView.Text = "Pending request...";
                    break;
                case RxStatus.Error:
                    _requestNetworkFetchButton.Enabled = true;
                    _networkFetchTextView.Text = "Error has occured";
                    Toast.MakeText(Context, "No network", ToastLength.Short).Show();
                    break;
            }
        }

        public static SampleFragment NewInstance()
        {
            var bundle = new Bundle();
            var fragment = new SampleFragment { Arguments = bundle };
            return fragment;
        }
    }
}