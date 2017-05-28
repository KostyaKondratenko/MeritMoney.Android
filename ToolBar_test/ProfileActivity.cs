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
using System.IO;

namespace Merit_Money
{
    [Activity(Label = "ProfileActivity")]
    public class ProfileActivity : BaseBottomBarActivity,
        IDialogInterfaceOnClickListener,
        IDialogInterfaceOnCancelListener
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
        private bool AvatarWasChanged = false;

        private String SaveName = String.Empty;

        private bool SaveSwitchState = false;
        private bool ImagePickerWasPressed = false;

        public static readonly int PickImageId = 1000;
        public static readonly int RequestCropImage = 12;

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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MainToolbar.MenuItemClick += MainToolbar_MenuItemClick;
            UserAvatar.Click += UserAvatar_Click;
            GC.Collect();
        }

        private void InitializeProfile()
        {
            ProfileDatabase db = new ProfileDatabase();
            Profile p = db.GetProfile();

            UserName.Text = p.name;
            UserEmail.Text = p.email;
            NotificationSwitch.Checked = p.emailNotificaion;

            new CacheUserAvatar(UserAvatar, Application.Context).Execute(p);
        }

        private void SetSwitchState()
        {
            if (NotificationSwitch.Checked) { SwitchState.Text = "On"; }
            else { SwitchState.Text = "Off"; }
        }

        private void UserAvatar_Click(object sender, EventArgs e)
        {
            if (isEditing && !ImagePickerWasPressed)
            {
                ImagePickerWasPressed = true;
                HideKeyboard(EditName);
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Avatar"), PickImageId);
            }
        }

        private async void MainToolbar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
        {
            ImagePickerWasPressed = false;

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
                            dialog.SetMessage(GetString(Resource.String.NoInternet));
                            dialog.SetCancelable(true);
                            dialog.SetPositiveButton("OK", this);
                            dialog.Create().Show();

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

            if (AvatarWasChanged)
            {
                new UploadAvatarOnServer(UserAvatar).Execute().Get();
                AvatarWasChanged = false;
            }

            if (SwitchWasChanged && !nameWasChanged)
            {
                Profile p = await MeritMoneyBrain.updateProfile(String.Empty, SwitchWasChanged, NotificationSwitch.Checked);
                p.AvatarIsDefault = OperationWithBitmap.isDefault(p.imageUri);
                ProfileDatabase db = new ProfileDatabase();
                db.Update(p);
            }
            if (nameWasChanged)
            {
                Profile p = await MeritMoneyBrain.updateProfile(UserName.Text, SwitchWasChanged, NotificationSwitch.Checked);
                ProfileDatabase db = new ProfileDatabase();
                db.Update(p);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                ImageCrop(uri);
                ImagePickerWasPressed = false;
            }

            if (requestCode == RequestCropImage) {
                Bundle extras = data.Extras;
                if (extras != null)
                {
                    Bitmap photo = extras.GetParcelable("data") as Bitmap;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        photo.Compress(Bitmap.CompressFormat.Jpeg, 75, stream);
                    }
                    UserAvatar.SetImageBitmap(photo);
                    AvatarWasChanged = true;
                }
            }
            
        }

        public void ImageCrop(Android.Net.Uri uri)
        {
            try
            {
                var CropIntent = new Intent("com.android.camera.action.CROP");

                CropIntent.SetDataAndType(uri, "image/*");

                CropIntent.PutExtra("crop", "true");
                CropIntent.PutExtra("outputX", 180);
                CropIntent.PutExtra("outputY", 180);
                CropIntent.PutExtra("aspectX", 1);
                CropIntent.PutExtra("aspectY", 1);
                CropIntent.PutExtra("scaleUpIfNeeded", true);
                CropIntent.PutExtra("return-data", true);

                StartActivityForResult(CropIntent, RequestCropImage);

            }
            catch (ActivityNotFoundException) { }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.profile_top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            dialog.Dismiss();

            UserName.Text = SaveName;
            EditName.Text = String.Empty;

            NotificationSwitch.Checked = SaveSwitchState;

            EditingItemsVisible(ViewStates.Invisible, ViewStates.Visible);
            isEditing = false; 
        }

        public void OnCancel(IDialogInterface dialog)
        {
            ShowKeyboard(EditName);
        }

        private class UploadAvatarOnServer : AsyncTask<Java.Lang.Void, Java.Lang.Void, Task<Bitmap>>
        {
            private CircularImageView image;
            private String UserId;
            private ProfileDatabase db = new ProfileDatabase();
            private String url;

            public UploadAvatarOnServer(CircularImageView image)
            {
                this.image = image;
                UserId = db.GetProfile().ID;
            }

            protected override void OnPreExecute()
            {
                base.OnPreExecute();
                OperationWithBitmap.Delete(Application.Context, UserId);
            }

            protected override async Task<Bitmap> RunInBackground(params Java.Lang.Void[] @params)
            {
                Android.Graphics.Drawables.BitmapDrawable drawable =
                    (Android.Graphics.Drawables.BitmapDrawable)image.Drawable;
                Bitmap bitmap = drawable.Bitmap;
                url = await MeritMoneyBrain.UploadImage(bitmap);

                return bitmap;
            }

            protected override void OnPostExecute(Task<Bitmap> result)
            {
                byte[] bitmapData = OperationWithBitmap.ConvertToByteArray(result.Result);
                db.UpdateAvatarUrl(New: url, at: UserId);
                OperationWithBitmap.Cache(Application.Context, bitmapData, UserId);
             
                base.OnPostExecute(result);
            }
        }
    }
}