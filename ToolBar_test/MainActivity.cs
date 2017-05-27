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
using Android.Support.V4.Widget;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;

namespace Merit_Money
{
    [Activity(Label = "Merit Money")]
    public class MainActivity : BaseBottomBarActivity,
        IDialogInterfaceOnClickListener
    {
        private SupportToolBar MainToolbar;
        private Button SayThanksButton;
        private SwipeRefreshLayout RefreshInfo;
        public static bool Loggedin;

        private TextView UserName;
        private TextView UserEmail;
        private TextView Balance;
        private TextView Rewards;
        private TextView Distribute;
        private CircularImageView UserAvatar;
        private TextView Initials;

        private TextView ABPointstext;
        private TextView RPointstext;
        private TextView CDPointstext;

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
            Initials = FindViewById<TextView>(Resource.Id.Initials);

            ABPointstext = FindViewById<TextView>(Resource.Id.ABpointsText);
            RPointstext = FindViewById<TextView>(Resource.Id.RpointsText);
            CDPointstext = FindViewById<TextView>(Resource.Id.CDpointsText);

            ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            Loggedin = info.GetBoolean(GetString(Resource.String.LogIn), false);

            if (!Loggedin)
            {
                Intent LogInIntent = new Intent(this, typeof(LogInActivity));
                LogInIntent.AddFlags(ActivityFlags.NoAnimation);
                this.StartActivityForResult(LogInIntent, LOG_IN_REQUEST);
                OverridePendingTransition(0, 0);
            }
            else
            {
                InitializeProfile();
            }

            CorrectPointsText();

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SayThanksButton.SetBackgroundResource(Resource.Color.button_ripple);
            }

            MainToolbar.InflateMenu(Resource.Menu.top_menu);
            MainToolbar.SetLogo(Resource.Drawable.ic_cloud_queue_white_24dp);
            MainToolbar.Title = "Merit Money";
            MainToolbar.TitleMarginStart = AdditionalFunctions.ConvertDpToPx(40);

            SayThanksButton.Click += SayThanksButton_Click;
            MainToolbar.MenuItemClick += MainToolbar_MenuItemClick;
            RefreshInfo.Refresh += Profile_Refresh;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SayThanksButton.Click -= SayThanksButton_Click;
            MainToolbar.MenuItemClick -= MainToolbar_MenuItemClick;
            RefreshInfo.Refresh -= Profile_Refresh;
            GC.Collect();
        }

        private async void Profile_Refresh(object sender, EventArgs e)
        {
            if (NetworkStatus.State != NetworkState.Disconnected)
            {
                Profile profile = await MeritMoneyBrain.GetProfile();
                await new CheckWhetherAvatarIsDefault(profile).Execute().GetAsync();
                ProfileDatabase db = new ProfileDatabase();
                db.Update(profile);
                InitializeProfile();
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.NoInternet), ToastLength.Short).Show();
            }
            RefreshInfo.Refreshing = false;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == LOG_IN_REQUEST)
            {
                if (resultCode == Result.Ok)
                {
                    Loggedin = data.GetBooleanExtra(GetString(Resource.String.LogIn), false);
                    Thread thread = new Thread(() =>
                    {
                        ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                        ISharedPreferencesEditor edit = info.Edit();
                        edit.PutBoolean(GetString(Resource.String.LogIn), Loggedin);
                        edit.Apply();
                    });
                    thread.Start();
                    InitializeProfile();
                }
            }
        }

        private void InitializeProfile()
        {
            ProfileDatabase db = new ProfileDatabase();
            Profile p = db.GetProfile();

            UserName.Text = p.name;
            UserEmail.Text = p.email;
            Balance.Text = p.balance.ToString();
            Rewards.Text = p.rewards.ToString();
            Distribute.Text = p.distribute.ToString();
            Initials.Text = AdditionalFunctions.DefineInitials(p.name);

            new CacheUserAvatar(UserAvatar, Initials, Application.Context).Execute(p);
        }

        private async void MainToolbar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.menu_logout:

                    ProgressDialog dialog = ProgressDialog.Show(this, "", "Logging out...");

                    await MeritMoneyBrain.LogOut();

                    ProfileDatabase pdb = new ProfileDatabase();
                    pdb.DeleteDatabase();

                    UsersDatabase udb = new UsersDatabase();
                    udb.DeleteDatabase();

                    ISharedPreferences info = Application.Context.GetSharedPreferences(Application.Context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                    ISharedPreferencesEditor editor = info.Edit();
                    editor.Clear();
                    editor.Apply();

                    dialog.Dismiss();

                    Intent LogInIntent = new Intent(this, typeof(LogInActivity));
                    this.StartActivityForResult(LogInIntent, LOG_IN_REQUEST);
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

        private void CorrectPointsText()
        {
            try
            {
                if (Convert.ToInt32(Distribute.Text) == 1) { CDPointstext.Text = "point"; }
                else { CDPointstext.Text = "points"; }

                if (Convert.ToInt32(Balance.Text) == 1) { ABPointstext.Text = "point"; }
                else { ABPointstext.Text = "points"; }

                if (Convert.ToInt32(Rewards.Text) == 1) { RPointstext.Text = "point"; }
                else { RPointstext.Text = "points"; }

            }
            catch (OverflowException e) { Console.Out.WriteLine(e.Message); }

        }

        private class CheckWhetherAvatarIsDefault : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
        {
            private Profile profile;

            public CheckWhetherAvatarIsDefault(Profile profile)
            {
                this.profile = profile;
            }

            protected override Java.Lang.Void RunInBackground(params Java.Lang.Void[] @params)
            {
                profile.AvatarIsDefault = OperationWithBitmap.isDefault(profile.imageUri);

                return null;
            }
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            dialog.Dismiss();
        }
    }
}

