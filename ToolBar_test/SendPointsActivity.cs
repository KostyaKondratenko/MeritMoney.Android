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
using Merit_Money;
using System.Text.RegularExpressions;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;
using SupportEditText = Android.Support.Design.Widget.TextInputEditText;

namespace Merit_Money
{
    [Activity(Label = "SendPointsActivity")]
    public class SendPointsActivity : BaseBottomBarActivity, 
        IDialogInterfaceOnClickListener
    {
        private Button SendPointsButton;
        private SupportToolBar ToolBar;
        private SupportEditText userNameToDistribute;
        private SupportEditText NumberOfPoints;
        private SupportEditText Notes;
        private String userIDtoDistribute;
        private TextView CanDistributePoints;

        private static readonly int SELECT_PERSON_REQUEST = 5;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FrameLayout SendPointsLayout = new FrameLayout(this);
            SetContentView(SendPointsLayout);
            base.CombineWith(SendPointsLayout, Resource.Layout.SendPoints, ActivityIs.Home);

            SendPointsButton = FindViewById<Button>(Resource.Id.SPSendButton);
            ToolBar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            userNameToDistribute = FindViewById<SupportEditText>(Resource.Id.SPSelectPerson);
            NumberOfPoints = FindViewById<SupportEditText>(Resource.Id.SPNumOfPointsEditText);
            CanDistributePoints = FindViewById<TextView>(Resource.Id.SPCDpoints);
            Notes = FindViewById<SupportEditText>(Resource.Id.SPNotesEditText);

            SendPointsButton.Enabled = false;

            InitializeProfile();

            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                SendPointsButton.SetBackgroundResource(Resource.Color.button_ripple);
            }

            SetSupportActionBar(ToolBar);
            SupportActionBar.Title = "Send Points";

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SendPointsButton.Click += SendPointsButton_Clicked;
            userNameToDistribute.Click += SelectPerson_Clicked;
            NumberOfPoints.FocusChange += NumberOfPoints_FocusChanged;
            SendPointsLayout.Touch += Layout_Touched;
        }

        private void Layout_Touched(object sender, View.TouchEventArgs e)
        {
            HideKeyboard(NumberOfPoints);
            NumberOfPoints.ClearFocus();
            Notes.ClearFocus();
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private void InitializeProfile()
        {
            ProfileDatabase db = new ProfileDatabase(GetString(Resource.String.ProfileDBFilename));
            Profile p = db.GetProfile();
            CanDistributePoints.Text = p.distribute.ToString();
        }

        private void NumberOfPoints_FocusChanged(object sender, View.FocusChangeEventArgs e)
        {
            if(NumberOfPoints.Text!=String.Empty && userNameToDistribute.Text != String.Empty)
            {
                SendPointsButton.Enabled = true;
            }else
            {
                SendPointsButton.Enabled = false;
            }
            if (NumberOfPoints.Text != String.Empty && NumberOfPoints.Text[0] == '0' )
            {
                NumberOfPoints.Text = Regex.Replace(NumberOfPoints.Text, @"^0+", "");
            }
        }

        private void SelectPerson_Clicked(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SearchPersonActivity));
            this.StartActivityForResult(intent, SELECT_PERSON_REQUEST);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == SELECT_PERSON_REQUEST)
            {
                if (resultCode == Result.Ok)
                {
                    userIDtoDistribute = data.GetStringExtra(GetString(Resource.String.ID));
                    userNameToDistribute.Text = data.GetStringExtra(GetString(Resource.String.UserName));
                }
            }
        }

        private void SendPointsButton_Clicked(object sender, EventArgs e)
        {
            if (NumberOfPoints.Text != String.Empty && userNameToDistribute.Text != String.Empty)
            {
                Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                dialog.SetMessage("Do you want to send " + NumberOfPoints.Text + " point(s) to " + userNameToDistribute.Text + "?");
                dialog.SetCancelable(true);
                dialog.SetNeutralButton("Cancel", this);
                dialog.SetPositiveButton("Send", this);
                dialog.Create().Show();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public async void OnClick(IDialogInterface dialog, int which)
        {
            switch (which)
            {
                case (int)DialogButtonType.Neutral:
                    dialog.Dismiss();
                    NumberOfPoints.Text = String.Empty;
                    SendPointsButton.Enabled = false;
                    break;
                case (int)DialogButtonType.Positive:
                    ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Sending points...", true);

                    String name = userNameToDistribute.Text;
                    userNameToDistribute.Text = String.Empty;
                    String number = NumberOfPoints.Text;
                    NumberOfPoints.Text = String.Empty;
                    String notes = Notes.Text;

                    await MeritMoneyBrain.DistributePoints(number, userIDtoDistribute, notes);
                    
                    Profile profile = await MeritMoneyBrain.GetProfile();
                    new UpdateProfileData(this, progressDialog, number, name).Execute(profile);
                    break;
            }
        }

        private class UpdateProfileData : AsyncTask<Profile, Java.Lang.Void, Java.Lang.Void>
        {
            private Activity context;
            private ProgressDialog progressDialog;
            String NumberOfPoints;
            String userNameToDistribute;

            public UpdateProfileData(Activity context, ProgressDialog dialog, String NumberOfPoints, String userNameToDistribute)
            {
                this.progressDialog = dialog;
                this.context = context;
                this.NumberOfPoints = NumberOfPoints;
                this.userNameToDistribute = userNameToDistribute;
            }

            protected override void OnPreExecute()
            { 
                base.OnPreExecute();
            }

            protected override Java.Lang.Void RunInBackground(params Profile[] @params)
            {
                Profile profile = @params[0];
                ProfileDatabase db = new ProfileDatabase(context.GetString(Resource.String.ProfileDBFilename));
                db.Update(profile);
                return null;
            }

            protected override void OnPostExecute(Java.Lang.Void result)
            {
                progressDialog.Dismiss();
                String points = (NumberOfPoints == "1") ? " point was" : " points were";
                Intent refresh = new Intent(context, typeof(SendPointsActivity));
                refresh.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.NoAnimation);
                context.StartActivity(refresh);
                context.Finish();
                context.OverridePendingTransition(0, 0);
                Toast.MakeText(context, NumberOfPoints + points +" sent to " + userNameToDistribute, ToastLength.Short).Show();
                base.OnPostExecute(result);
            }
        }
    }
}