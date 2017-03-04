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
using SupportToolBar = Android.Support.V7.Widget.Toolbar;

namespace ToolBar_test
{
    [Activity(Label = "SendPointsActivity")]
    public class SendPointsActivity : AppCompatActivity
    {
        private Button SendPointsButton;
        private SupportToolBar ToolBar;
        private AutoCompleteTextView peopleList;
        private EditText NumberOfPoints;

        static string[] TEST = new string[] { "ONE", "TWO", "THREE", "FOUR", "FIVE" };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SendPoints);

            SendPointsButton = FindViewById<Button>(Resource.Id.SPSendButton);
            ToolBar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            peopleList = FindViewById<AutoCompleteTextView>(Resource.Id.SPSelectPerson);
            NumberOfPoints = FindViewById<EditText>(Resource.Id.SPNumOfPointsEditText);

            //SendPointsButton.Enabled = false;

            SetSupportActionBar(ToolBar);
            SupportActionBar.Title = "Send Points";

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            //T = User Class
            var AdapterOfPeople = new ArrayAdapter<String>(this, Resource.Layout.list_item, TEST);

            peopleList.Adapter = AdapterOfPeople;
            SendPointsButton.Click += SendPointsButton_Clicked;
        }

        //XML: android:onClick="SelectPerson_Clicked"
        
        private void SelectPerson_Clicked(object sender, EventArgs e)
        {
            //Intent intent = new Intent(this, typeof(SearchPersonActivity));
            //this.StartActivity(intent);
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
            Intent intent = new Intent(this, typeof(SearchPersonActivity));
            this.StartActivity(intent);
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