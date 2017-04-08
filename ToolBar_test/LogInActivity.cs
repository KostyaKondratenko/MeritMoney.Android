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
using System.Json;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Android.Support.V7.App;
using ToolBar_test;

namespace Merit_Money
{
    [Activity(Label = "LogInActivity")]
    public class LogInActivity : AppCompatActivity
    {
        private EditText Email;
        private Button LogIn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LogInWithEmail);

            Email = FindViewById<EditText>(Resource.Id.LogInViaEmail);
            LogIn = FindViewById<Button>(Resource.Id.LogInButton);
            
            LogIn.Click += LogIn_Clicked;
        }

        private async void LogIn_Clicked(object sender, EventArgs e)
        {
            if (Email.Text != String.Empty)
            {
                await MeritMoneyBrain.AccessToken(Email.Text);
                Profile profile = await MeritMoneyBrain.GetProfile();
                Intent returnIntent = new Intent();
                returnIntent.PutExtra(GetString(Resource.String.LogIn), true);
                SetResult(Result.Ok, returnIntent);
                SaveData(profile);
                Finish();
            }
        }

        private void SaveData(Profile profile)
        {
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
        }
    }
}