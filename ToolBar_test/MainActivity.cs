using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.Threading;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.App;
using System.Json;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using ToolBar_test;
using Android.Support.V4.Widget;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;

namespace Merit_Money
{
    [Activity(Label = "Merit Money", MainLauncher = true, Icon = "@drawable/cloud")]
    public class MainActivity : BaseBottomBarActivity
    {
        private SupportToolBar MainToolbar;
        private Button SayThanksButton;
        //private ProfileClass profile;
        private SwipeRefreshLayout RefreshInfo;
        public static bool Loggedin;

        private TextView UserName;
        private TextView UserEmail;
        private TextView Balance;
        private TextView Rewards;
        private TextView Distribute;
        private CircularImageView UserAvatar;

        private static readonly int LOG_IN_REQUEST = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            FrameLayout MainLayout = new FrameLayout(this);
            SetContentView(MainLayout);
            base.CombineWith(MainLayout, Resource.Layout.Main, ActivityIs.Home);

            MainToolbar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            SayThanksButton = FindViewById<Button>(Resource.Id.thanksButton);
            UserAvatar = FindViewById<CircularImageView>(Resource.Id.UserAvatar);
            UserName = FindViewById<TextView>(Resource.Id.UserName);
            UserEmail = FindViewById<TextView>(Resource.Id.UserEmail);
            Balance = FindViewById<TextView>(Resource.Id.ABpoints);
            Rewards = FindViewById<TextView>(Resource.Id.Rpoints);
            Distribute = FindViewById<TextView>(Resource.Id.CDpoints);
            RefreshInfo = FindViewById<SwipeRefreshLayout>(Resource.Id.activity_main_swipe_refresh_layout);

            ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            Loggedin = info.GetBoolean(GetString(Resource.String.LogIn), false);

            if (!Loggedin)
            {
                Intent LogInIntent = new Intent(this, typeof(LogInActivity));
                this.StartActivityForResult(LogInIntent, LOG_IN_REQUEST);
            }
            else
            {
                Thread thread = new Thread(() =>
                {
                    InitializeProfile();
                });
                thread.Start();
            }

            //Correct "point(s)" textView

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SayThanksButton.SetBackgroundResource(Resource.Color.button_ripple);
            }

            MainToolbar.InflateMenu(Resource.Menu.top_menu);
            MainToolbar.SetLogo(Resource.Drawable.ic_cloud_queue_white_24dp);
            MainToolbar.Title = "Merit Money";
            MainToolbar.TitleMarginStart = base.ConvertDpToPx(40);

            SayThanksButton.Click += SayThanksButton_Click;
            MainToolbar.MenuItemClick += MainToolbar_MenuItemClick;
            RefreshInfo.Refresh += Profile_Refresh;
        }

        private async void Profile_Refresh(object sender, EventArgs e)
        {
            Profile profile = await MeritMoneyBrain.GetProfile();
            ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            ISharedPreferencesEditor edit = info.Edit();
            edit.PutString(GetString(Resource.String.UserName), profile.name);
            edit.PutString(GetString(Resource.String.UserEmail), profile.email);
            edit.PutString(GetString(Resource.String.UserAvatar), profile.imageUri);
            edit.PutInt(GetString(Resource.String.BalancePoints), profile.balance);
            edit.PutInt(GetString(Resource.String.RewardsPoints), profile.rewards);
            edit.PutInt(GetString(Resource.String.DistributePoints), profile.distribute);
            edit.PutString(GetString(Resource.String.CurrentAccessToken), MeritMoneyBrain.CurrentAccessToken);
            edit.PutBoolean(GetString(Resource.String.EmailNotification), profile.emailNotificaion);
            edit.Apply();
            InitializeProfile();
            RefreshInfo.Refreshing = false;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == LOG_IN_REQUEST)
            {
                // Make sure the request was successful
                if (resultCode == Result.Ok)
                {
                    Loggedin = data.GetBooleanExtra(GetString(Resource.String.LogIn), false);
                    Thread thread = new Thread(() =>
                    {
                        ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                        ISharedPreferencesEditor edit = info.Edit();
                        edit.PutBoolean(GetString(Resource.String.LogIn), Loggedin);
                        edit.Apply();
                        InitializeProfile();
                    });
                    thread.Start();
                }
            }
        }

        private void InitializeProfile()
        {
            ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            UserName.Text = info.GetString(GetString(Resource.String.UserName), String.Empty);
            UserEmail.Text = info.GetString(GetString(Resource.String.UserEmail), String.Empty);
            Balance.Text = info.GetInt(GetString(Resource.String.BalancePoints), -1).ToString();
            Rewards.Text = info.GetInt(GetString(Resource.String.RewardsPoints), -1).ToString();
            Distribute.Text = info.GetInt(GetString(Resource.String.DistributePoints), -1).ToString();
            MeritMoneyBrain.CurrentAccessToken = info.GetString(GetString(Resource.String.CurrentAccessToken), String.Empty);

            //this.RunOnUiThread(() => {
            //    var imageBitmap = base.GetImageBitmapFromUrl(info.GetString(GetString(Resource.String.UserAvatar), String.Empty));
            //    if (imageBitmap != null)
            //    {
            //        UserAvatar.SetImageBitmap(imageBitmap);
            //    }
            //    else
            //    {
            //        UserAvatar.SetImageResource(Resource.Drawable.ic_noavatar);
            //    }
            //});

            //if (UserName.Text == String.Empty || UserEmail.Text == String.Empty ||
            //    Balance.Text == "-1" || Rewards.Text == "-1" || Distribute.Text == "-1")
            //{
            //    return false;
            //}

            //return true;
        }

        private async void MainToolbar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
        {
            //Add a logout method
            switch (e.Item.ItemId)
            {
                case Resource.Id.menu_logout:
                    //Toast.MakeText(this, "Action selected: " + e.Item.TitleFormatted, ToastLength.Short).Show();
                    await MeritMoneyBrain.LogOut();
                    break;
            }
        }

        private void SayThanksButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SendPointsActivity));
            this.StartActivity(intent);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
    }
}

