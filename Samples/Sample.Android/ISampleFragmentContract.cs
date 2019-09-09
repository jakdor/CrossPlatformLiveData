using Sample.Core.ViewModel;

namespace Sample.Android
{
    internal interface ISampleFragmentContract
    {
        ISampleViewModel GetSampleViewModel();
    }
}