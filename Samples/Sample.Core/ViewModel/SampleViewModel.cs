using System;
using System.Threading.Tasks;
using CrossPlatformLiveData;
using Sample.Core.Model;
using Timer = System.Timers.Timer;

namespace Sample.Core.ViewModel
{
    public class SampleViewModel : ISampleViewModel
    {
        public ILiveData<string> ClockLiveData { get; } = new LiveData<string>();
        public ILiveData<RxWrapper<SampleResponse>> FakeNetworkingLiveData { get; set; } =
            new LiveData<RxWrapper<SampleResponse>>();

        private Timer _timer;
        private readonly Random _randomGen = new Random();
        private int _fakeNetworkSequence;

        public SampleViewModel()
        {
            SetupTimer();
        }

        ~SampleViewModel()
        {
            _timer.Dispose();
        }

        /// <summary>
        /// Start time updates
        /// </summary>
        public void StartClock()
        {
            if(!_timer.Enabled) _timer.Start();
        }

        /// <summary>
        /// Stop time updates
        /// </summary>
        public void StopClock()
        {
            if (_timer.Enabled) _timer.Stop();
        }

        /// <summary>
        /// Simulated network fetch
        /// Error every third call
        /// </summary>
        public void GetFakeNetworking()
        {
            //Check if request is not already pending
            if (FakeNetworkingLiveData.Value.Status != RxStatus.Pending)
            {
                //Notify UI that request is pending
                FakeNetworkingLiveData.PostValue(RxWrapper<SampleResponse>.Pending());

                //Fake request latency 0.5-2s
                Task.Delay(_randomGen.Next(500, 2000)).ContinueWith(task =>
                {
                    switch (_fakeNetworkSequence)
                    {
                        case 0:
                            var model1 = new SampleResponse
                            {
                                Id = 1,
                                Name = "Ok response 1"
                            };

                            //Post fake fetch data to UI
                            FakeNetworkingLiveData.PostValue(RxWrapper<SampleResponse>.Ok(model1));
                            break;
                        case 1:
                            var model2 = new SampleResponse
                            {
                                Id = 2,
                                Name = "Ok response 2"
                            };

                            //Post fake fetch data to UI
                            FakeNetworkingLiveData.PostValue(RxWrapper<SampleResponse>.Ok(model2));
                            break;
                        case 2:

                            //Fake error has occured during network call
                            FakeNetworkingLiveData.PostValue(RxWrapper<SampleResponse>.Error(new Exception("No network")));
                            break;
                    }

                    ++_fakeNetworkSequence;
                    if (_fakeNetworkSequence > 2) _fakeNetworkSequence = 0;
                });
            }
        }

        /// <summary>
        /// Setup timer that will post new formatted string with current time to UI every second
        /// </summary>
        private void SetupTimer()
        {
            _timer = new Timer
            {
                Interval = 1000
            };

            _timer.Elapsed += (sender, args) => ClockLiveData.PostValue(FormatClock());
            _timer.Start();
        }

        /// <summary>
        /// Format current time
        /// </summary>
        /// <returns></returns>
        private string FormatClock()
        {
            var hh = DateTime.Now.Hour;
            var mm = DateTime.Now.Minute;
            var ss = DateTime.Now.Second;

            var time = "";

            if (hh < 10)
            {
                time += "0" + hh;
            }
            else
            {
                time += hh;
            }
            time += ":";

            if (mm < 10)
            {
                time += "0" + mm;
            }
            else
            {
                time += mm;
            }
            time += ":";

            if (ss < 10)
            {
                time += "0" + ss;
            }
            else
            {
                time += ss;
            }

            return time;
        }
    }
}
