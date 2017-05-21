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
    public class NoInternetActivity : BaseBottomBarActivity,
        IDialogInterfaceOnClickListener
    {
        private Button TryAgainButton;
        private SupportToolBar MainToolbar;
        private View LoadingPanel;
        private TextView NoInternetText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.NoInternet);

            Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            dialog.SetMessage(GetString(Resource.String.NoInternet));
            dialog.SetNeutralButton("OK", this);
            dialog.Create().Show();

            MainToolbar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            TryAgainButton = FindViewById<Button>(Resource.Id.RetryButton);
            LoadingPanel = FindViewById<View>(Resource.Id.loadingPanel);
            NoInternetText = FindViewById<TextView>(Resource.Id.NoInternetText);

            LoadingPanel.Visibility = ViewStates.Invisible;

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                TryAgainButton.SetBackgroundResource(Resource.Color.button_ripple);
            }

            MainToolbar.SetLogo(Resource.Drawable.ic_cloud_queue_white_24dp);
            MainToolbar.Title = "Merit Money";
            MainToolbar.TitleMarginStart = base.ConvertDpToPx(40);

            TryAgainButton.Click += TryAgainButton_Click;
        }

        private async void TryAgainButton_Click(object sender, EventArgs e)
        {
            LoadingPanel.Visibility = ViewStates.Visible;
            NoInternetText.Visibility = ViewStates.Invisible;

            await System.Threading.Tasks.Task.Delay(100);

            if (NetworkStatus.State != NetworkState.Disconnected)
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            }
            else
            {
                LoadingPanel.Visibility = ViewStates.Invisible;
                NoInternetText.Visibility = ViewStates.Visible;
            }
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            switch (which) {
                case (int)DialogButtonType.Neutral:
                    dialog.Dismiss();
                    break;
        }
        }
    }
}