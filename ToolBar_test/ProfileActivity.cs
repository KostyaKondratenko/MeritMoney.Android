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
using Android.Views.InputMethods;
using System.Threading;
using ToolBar_test;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;

namespace Merit_Money
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : BaseBottomBarActivity
    {
        private SupportToolBar MainToolbar;
        private bool isEditing = false;
        private TextView SwitchState;
        private EditText EditName;

        private TextView UserName;
        private CircularImageView UserAvatar;
        private TextView UserEmail;
        private Switch NotificationSwitch;

        public static readonly int PickImageId = 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FrameLayout ProfileLayout = new FrameLayout(this);
            SetContentView(ProfileLayout);
            base.CombineWith(ProfileLayout, Resource.Layout.Profile, ActivityIs.Profile);

            MainToolbar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            EditName = FindViewById<EditText>(Resource.Id.EditProfileUserName);
            SwitchState = FindViewById<TextView>(Resource.Id.SwitchStateText);

            NotificationSwitch = FindViewById<Switch>(Resource.Id.SwitchEmailNotifications);
            UserAvatar = FindViewById<CircularImageView>(Resource.Id.UserAvatar);
            UserName = FindViewById<TextView>(Resource.Id.ProfileUserName);
            UserEmail = FindViewById<TextView>(Resource.Id.ProfileUserEmail);

            Thread thread = new Thread(() =>
            {
                InitializeProfile();
            });
            thread.Start();

            MainToolbar.InflateMenu(Resource.Menu.profile_top_menu);
            MainToolbar.Title = "Profile";

            NotificationSwitch.Visibility = ViewStates.Invisible;
            EditName.Visibility = ViewStates.Invisible;
            SetSwitchState();

            MainToolbar.MenuItemClick += MainToolbar_MenuItemClick;
            UserAvatar.Click += UserAvatar_Click;
        }

        public override void OnBackPressed()
        {
            if (DoubleClickedToExit)
            {
                base.OnBackPressed();
                Java.Lang.JavaSystem.Exit(0);
                return;
            }

            this.DoubleClickedToExit = true;
            Toast.MakeText(this, "Press Back to Exit", ToastLength.Short).Show();

            new Handler().PostDelayed(() =>
            {
                DoubleClickedToExit = false;
            }, 3000);
        }

        private void InitializeProfile()
        {
            ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            UserName.Text = info.GetString(GetString(Resource.String.UserName), String.Empty);
            UserEmail.Text = info.GetString(GetString(Resource.String.UserEmail), String.Empty);
            NotificationSwitch.Checked = info.GetBoolean(GetString(Resource.String.EmailNotification), false);
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
        }

        private void SetSwitchState()
        {
            if (NotificationSwitch.Checked) { SwitchState.Text = "On"; }
            else { SwitchState.Text = "Off"; }
        }

        private void UserAvatar_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                HideKeyboard(EditName);
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Avatar"), PickImageId);
            }
        }

        private void MainToolbar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.menu_edit:
                    if (!isEditing)
                    {
                        e.Item.SetTitle("Save");

                        NotificationSwitch.Visibility = ViewStates.Visible;
                        EditName.Visibility = ViewStates.Visible;

                        SwitchState.Visibility = ViewStates.Invisible;
                        UserName.Visibility = ViewStates.Invisible;

                        EditName.Text = UserName.Text;

                        ShowKeyboard(EditName);
                        
                        isEditing = true;
                    }
                    else
                    {
                        e.Item.SetTitle("Edit");

                        NotificationSwitch.Visibility = ViewStates.Invisible;
                        EditName.Visibility = ViewStates.Invisible;

                        SwitchState.Visibility = ViewStates.Visible;
                        UserName.Visibility = ViewStates.Visible;

                        if (EditName.Text != String.Empty)
                        {
                            UserName.Text = EditName.Text;
                            EditName.Text = String.Empty;
                        }

                        HideKeyboard(EditName);

                        isEditing = false;
                    }
                    SetSwitchState();
                    break;
            }

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
            MenuInflater.Inflate(Resource.Menu.profile_top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void ShowKeyboard(EditText editText)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(editText, ShowFlags.Forced);
            imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            editText.SetSelection(editText.Text.Length);
        }

        private void HideKeyboard(EditText editText)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(editText.WindowToken, HideSoftInputFlags.None);
        }
    }
}