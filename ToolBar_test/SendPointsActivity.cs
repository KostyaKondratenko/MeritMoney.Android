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
using System.Text.RegularExpressions;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;
using SupportEditText = Android.Support.Design.Widget.TextInputEditText;

namespace Merit_Money
{
    [Activity(Label = "SendPointsActivity")]
    public class SendPointsActivity : BaseBottomBarActivity
    {
        private Button SendPointsButton;
        private SupportToolBar ToolBar;
        private SupportEditText peopleList;
        private SupportEditText NumberOfPoints;
        private TextView CanDistributePoints;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FrameLayout SendPointsLayout = new FrameLayout(this);
            SetContentView(SendPointsLayout);
            base.CombineWith(SendPointsLayout, Resource.Layout.SendPoints, ActivityIs.Home);

            SendPointsButton = FindViewById<Button>(Resource.Id.SPSendButton);
            ToolBar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            peopleList = FindViewById<SupportEditText>(Resource.Id.SPSelectPerson);
            NumberOfPoints = FindViewById<SupportEditText>(Resource.Id.SPNumOfPointsEditText);
            CanDistributePoints = FindViewById<TextView>(Resource.Id.SPCDpoints);
            //SendPointsButton.Enabled = false;

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
            peopleList.Click += SelectPerson_Clicked;
            NumberOfPoints.FocusChange += NumberOfPoints_FocusChanged;
        }

        private void InitializeProfile()
        {
            ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            CanDistributePoints.Text = info.GetInt(GetString(Resource.String.DistributePoints), -1).ToString();
        }

        private void NumberOfPoints_FocusChanged(object sender, View.FocusChangeEventArgs e)
        {
            if (NumberOfPoints.Text != String.Empty && NumberOfPoints.Text[0] == '0' )
            {
                NumberOfPoints.Text = Regex.Replace(NumberOfPoints.Text, @"^0+", "");
            }
        }

        private void SelectPerson_Clicked(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SearchPersonActivity));
            this.StartActivity(intent);
        }

        private void SendPointsButton_Clicked(object sender, EventArgs e)
        {
            //Add nuber of points, a person
            if (NumberOfPoints.Text != String.Empty && peopleList.Text != String.Empty)
            {
                Toast.MakeText(this, NumberOfPoints.Text + " points were sent to "+ peopleList.Text, ToastLength.Short).Show();
            }
            else if(NumberOfPoints.Text == String.Empty && peopleList.Text == String.Empty)
            {
                Toast.MakeText(this, "Please, fill in \"Select person\" and \"# of points\" fields.", ToastLength.Short).Show();
            }
            else if(NumberOfPoints.Text == String.Empty)
            {
                Toast.MakeText(this, "Please, fill in \"# of points\" field." + peopleList.Text, ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Please, fill in \"Select person\" field.", ToastLength.Short).Show();
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
    }
}