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
using System.Threading.Tasks;

namespace Merit_Money
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : BaseBottomBarActivity,
        IDialogInterfaceOnClickListener
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

            InitializeProfile();

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
            ProfileDatabase db = new ProfileDatabase(GetString(Resource.String.ProfileDBFilename));
            Profile p = db.GetProfile();

            UserName.Text = p.name;
            UserEmail.Text = p.email;
            NotificationSwitch.Checked = p.emailNotificaion;

            //Bitmap imageBitmap = MeritMoneyBrain.ReadFromInternalStorage(p.ID);
            //if (imageBitmap == null)
            //{
                new LoadAndSaveImage(UserAvatar).Execute(p.imageUri, p.ID);
            //}
            //else
            //{
            //    UserAvatar.SetImageBitmap(imageBitmap);
            //}
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
                        EditingItemsVisible(ViewStates.Visible, ViewStates.Invisible);

                        SaveName = UserName.Text;
                        SaveSwitchState = NotificationSwitch.Checked;

                        EditName.Text = UserName.Text;

                        ShowKeyboard(EditName);
                        isEditing = true;
                    }
                    else
                    {
                        if (NetworkStatus.State != NetworkState.Disconnected)
                        {
                            e.Item.SetTitle("Edit");

                            EditingItemsVisible(ViewStates.Invisible, ViewStates.Visible);
                            isEditing = false;
                            HideKeyboard(EditName);

                            ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Saving...", true);
                            await HandleResult();
                            progressDialog.Dismiss();
                        }
                        else
                        {
                            Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                            dialog.SetMessage("There is no Internet connection.");
                            dialog.SetCancelable(true);
                            dialog.SetPositiveButton("OK", this);
                            dialog.Create().Show();

                            UserName.Text = SaveName;
                            EditName.Text = String.Empty;

                            NotificationSwitch.Checked = SaveSwitchState;

                            EditingItemsVisible(ViewStates.Invisible, ViewStates.Visible);
                            isEditing = false;
                            HideKeyboard(EditName);
                        }
                    }
                    SetSwitchState();
                    break;
            }
        }

        private void EditingItemsVisible(ViewStates editingState, ViewStates savingState)
        {
            NotificationSwitch.Visibility = editingState;
            EditName.Visibility = editingState;

            SwitchState.Visibility = savingState;
            UserName.Visibility = savingState;
        }

        private async Task HandleResult()
        {
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

            if (SwitchWasChanged && !nameWasChanged)
            {
                Profile p = await MeritMoneyBrain.updateProfile(String.Empty, SwitchWasChanged, NotificationSwitch.Checked);
                ProfileDatabase db = new ProfileDatabase(GetString(Resource.String.ProfileDBFilename));
                db.Update(p);
            }
            if (nameWasChanged)
            {
                Profile p = await MeritMoneyBrain.updateProfile(UserName.Text, SwitchWasChanged, NotificationSwitch.Checked);
                ProfileDatabase db = new ProfileDatabase(GetString(Resource.String.ProfileDBFilename));
                db.Update(p);
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

        public void OnClick(IDialogInterface dialog, int which)
        {
            dialog.Dismiss();
        }
    }
}