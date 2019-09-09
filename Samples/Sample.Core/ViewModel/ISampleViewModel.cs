using CrossPlatformLiveData;
using Sample.Core.Model;

namespace Sample.Core.ViewModel
{
    public interface ISampleViewModel
    {
        ILiveData<string> ClockLiveData { get; }
        ILiveData<RxWrapper<SampleResponse>> FakeNetworkingLiveData { get; set; }

        void StartClock();
        void StopClock();
        void GetFakeNetworking();
    }
}
