using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;

namespace Merit_Money
{
    [Activity(MainLauncher = true, NoHistory = true, Icon = "@drawable/cloud")]
    public class SplashScreenActivity : BaseBottomBarActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SplashScreen);
        }

        public override void OnBackPressed() { }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        async void SimulateStartup()
        {
            await Task.Delay(500);
            ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            MeritMoneyBrain.CurrentAccessToken = info.GetString(GetString(Resource.String.CurrentAccessToken), String.Empty);
            if (NetworkStatus.State != NetworkState.Disconnected)
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            else
            {
               StartActivity(new Intent(Application.Context, typeof(NoInternetActivity)));
            }
        }
    }
}