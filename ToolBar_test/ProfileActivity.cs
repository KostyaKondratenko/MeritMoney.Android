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
using SupportToolBar = Android.Support.V7.Widget.Toolbar;
using System.Threading;
using Android.Graphics;

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

        private bool nameWasChanged = false;
        private bool SwitchWasChanged = false;
        private String SaveName = String.Empty;
        private bool SaveSwitchState = false;

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

            //Thread thread = new Thread(() =>
            //{
                InitializeProfile();
            //});
            //thread.Start();

            MainToolbar.InflateMenu(Resource.Menu.profile_top_menu);
            MainToolbar.Title = "Profile";

            NotificationSwitch.Visibility = ViewStates.Invisible;
            EditName.Visibility = ViewStates.Invisible;
            SetSwitchState();

            MainToolbar.MenuItemClick += MainToolbar_MenuItemClick;
            UserAvatar.Click += UserAvatar_Click;
        }


        private void InitializeProfile()
        {
            //ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            //NotificationSwitch.Checked = info.GetBoolean(GetString(Resource.String.EmailNotification), false);
            //UserName.Text = info.GetString(GetString(Resource.String.UserName), String.Empty);
            //UserEmail.Text = info.GetString(GetString(Resource.String.UserEmail), String.Empty);

            ProfileDatabase db = new ProfileDatabase(GetString(Resource.String.ProfileDBFilename));
            Profile p = db.GetProfile();

            UserName.Text = p.name;
            UserEmail.Text = p.email;
            NotificationSwitch.Checked = p.emailNotificaion;

            Bitmap imageBitmap = MeritMoneyBrain.ReadFromInternalStorage(p.ID);
            if (imageBitmap == null)
            {
                new LoadAndSaveImage(UserAvatar).Execute(p.imageUri, p.ID);
            }
            else
            {
                UserAvatar.SetImageBitmap(imageBitmap);
            }

            //new SetImageFromUrl(UserAvatar).Execute(info.GetString(GetString(Resource.String.UserAvatar), String.Empty));
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

        private async void MainToolbar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
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

                        SaveName = UserName.Text;
                        SaveSwitchState = NotificationSwitch.Checked;

                        EditName.Text = UserName.Text;

                        ShowKeyboard(EditName);

                        isEditing = true;
                    }
                    else
                    {
                        e.Item.SetTitle("Edit");

                        ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Saving...", true);

                        NotificationSwitch.Visibility = ViewStates.Invisible;
                        EditName.Visibility = ViewStates.Invisible;

                        SwitchState.Visibility = ViewStates.Visible;
                        UserName.Visibility = ViewStates.Visible;

                        if (EditName.Text != String.Empty)
                        {
                            UserName.Text = EditName.Text;
                            EditName.Text = String.Empty;
                        }

                        if (SaveSwitchState != NotificationSwitch.Checked)
                            SwitchWasChanged = true;
                        else
                            SwitchWasChanged = false;
                        if (SaveName != UserName.Text)
                            nameWasChanged = true;
                        else
                            nameWasChanged = false;

                        HideKeyboard(EditName);

                        isEditing = false;

                        if (SwitchWasChanged && !nameWasChanged)
                        {
                            Profile p = await MeritMoneyBrain.updateProfile(String.Empty, SwitchWasChanged, NotificationSwitch.Checked);
                            ProfileDatabase db = new ProfileDatabase(GetString(Resource.String.ProfileDBFilename));
                            db.Update(p);
                            progressDialog.Dismiss();
                        }
                        if (nameWasChanged)
                        {
                            Profile p = await MeritMoneyBrain.updateProfile(UserName.Text, SwitchWasChanged, NotificationSwitch.Checked);
                            ProfileDatabase db = new ProfileDatabase(GetString(Resource.String.ProfileDBFilename));
                            db.Update(p);
                            progressDialog.Dismiss();
                        }
                        progressDialog.Dismiss();
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
    }
}