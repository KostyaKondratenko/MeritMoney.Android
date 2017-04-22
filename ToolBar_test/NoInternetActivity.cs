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
using SupportToolBar = Android.Support.V7.Widget.Toolbar;

namespace Merit_Money
{
    [Activity(Label = "NoInternetActivity")]
    public class NoInternetActivity : BaseBottomBarActivity
    {
        private Button TryAgainButton;
        private SupportToolBar MainToolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.NoInternet);

            MainToolbar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            TryAgainButton = FindViewById<Button>(Resource.Id.RetryButton);

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                TryAgainButton.SetBackgroundResource(Resource.Color.button_ripple);
            }

            MainToolbar.SetLogo(Resource.Drawable.ic_cloud_queue_white_24dp);
            MainToolbar.Title = "Merit Money";
            MainToolbar.TitleMarginStart = base.ConvertDpToPx(40);

            TryAgainButton.Click += TryAgainButton_Click;
        }

        private void TryAgainButton_Click(object sender, EventArgs e)
        {
            if (NetworkStatus.State != NetworkState.Disconnected)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
        }
    }
}