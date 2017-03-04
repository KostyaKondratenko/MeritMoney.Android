using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V7.App;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;

namespace ToolBar_test
{
    [Activity(Label = "Merit Money", MainLauncher = true, Icon ="@drawable/cloud")]
    public class MainActivity : AppCompatActivity
    {
        private SupportToolBar MainToolbar;
        private Button SayThanksButton;
        private CircularImageView UserAvatar;
        public static readonly int PickImageId = 1000;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            MainToolbar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            SayThanksButton = FindViewById<Button>(Resource.Id.thanksButton);
            UserAvatar = FindViewById<CircularImageView>(Resource.Id.UserAvatar);

            MainToolbar.InflateMenu(Resource.Menu.top_menu);
            MainToolbar.SetLogo(Resource.Drawable.cloud);
            MainToolbar.Title = "   Merit Money";

            SayThanksButton.Click += SayThanksButton_Click;
            MainToolbar.MenuItemClick += MainToolbar_MenuItemClick;
            UserAvatar.Click += UserAvatar_Click;
        }

        private void UserAvatar_Click(object sender, EventArgs e)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Avatar"), PickImageId);
        }

        private void MainToolbar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
        {
            //Add a logout method
            switch (e.Item.ItemId)
            {
                case Resource.Id.menu_logout:
                    Toast.MakeText(this, "Action selected: " + e.Item.TitleFormatted, ToastLength.Short).Show();
                    break;
            }
        }

        private void SayThanksButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SendPointsActivity));
            this.StartActivity(intent);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                UserAvatar.SetImageURI(uri);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted, ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }
    }
}

