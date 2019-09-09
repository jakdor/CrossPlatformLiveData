using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Sample.Core.ViewModel;

namespace Sample.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISampleFragmentContract
    {
        /// <summary>
        /// In real application provide dependencies from core project with Dependency Injection or Dependency Container.
        /// Im keeping it simple for the sake of this sample.
        /// </summary>
        private static readonly ISampleViewModel SampleViewModel = new SampleViewModel();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if (savedInstanceState == null)
            {
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.fragment_layout, SampleFragment.NewInstance())
                    .Commit();
            }
        }

        public ISampleViewModel GetSampleViewModel()
        {
            return SampleViewModel;
        }
    }
}