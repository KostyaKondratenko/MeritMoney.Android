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
using Merit_Money;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Plus;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;

namespace Merit_Money
{
    [Activity(Label = "LogInActivity")]
    public class LogInActivity : AppCompatActivity, GoogleApiClient.IOnConnectionFailedListener
    {
        private SignInButton LogIn;
        private GoogleApiClient GoogleClient;
        private Button tmpEmailLogIn;

        private readonly int SIGN_IN = 7;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LogInWithEmail);

            tmpEmailLogIn = FindViewById<Button>(Resource.Id.tmpLogInWithEmail);
            LogIn = FindViewById<SignInButton>(Resource.Id.LogInButton);
            SetGoogleSingInButtonText(LogIn, "Log in with Google");

            LogIn.Click += LogIn_Clicked;
            tmpEmailLogIn.Click += tmp_Click;

            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken(GetString(Resource.String.ServerClientID))
                    .Build();


            GoogleClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();
        }

        private async void tmp_Click(object sender, EventArgs e)
        {
            //String email = "kondratenkokostya@gmail.com";
            String email = "intellogic.ukr@gmail.com";
            if (email != String.Empty)
            {
                await MeritMoneyBrain.AccessToken(email);
                Profile profile = await MeritMoneyBrain.GetProfile();
                Intent returnIntent = new Intent();
                returnIntent.PutExtra(GetString(Resource.String.LogIn), true);
                SetResult(Result.Ok, returnIntent);
                SaveData(profile);
                Finish();
            }
        }

        private void LogIn_Clicked(object sender, EventArgs e)
        {
            //if (Email.Text != String.Empty)
            //{
            //    await MeritMoneyBrain.AccessToken(Email.Text);
            //    Profile profile = await MeritMoneyBrain.GetProfile();
            //    Intent returnIntent = new Intent();
            //    returnIntent.PutExtra(GetString(Resource.String.LogIn), true);
            //    SetResult(Result.Ok, returnIntent);
            //    SaveData(profile);
            //    Finish();
            //}

            Intent signInIntent = Auth.GoogleSignInApi.GetSignInIntent(GoogleClient);
            StartActivityForResult(signInIntent, SIGN_IN);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                if (requestCode == SIGN_IN)
                {
                    GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                    HandleSignInResult(result);
                }
            }
        }

        private async void HandleSignInResult(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                GoogleSignInAccount acct = result.SignInAccount;
                await MeritMoneyBrain.SingInWithGoogle(acct.IdToken);
                //updateUI(true);
            }
            else
            {
                // Signed out, show unauthenticated UI.
                //updateUI(false);
            }
        }

        private void SaveData(Profile profile)
        {
            ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            ISharedPreferencesEditor edit = info.Edit();
            //edit.PutString(GetString(Resource.String.ID), profile.ID);
            //edit.PutString(GetString(Resource.String.UserName), profile.name);
            //edit.PutString(GetString(Resource.String.UserEmail), profile.email);
            //edit.PutString(GetString(Resource.String.UserAvatar), profile.imageUri);
            //edit.PutInt(GetString(Resource.String.BalancePoints), profile.balance);
            //edit.PutInt(GetString(Resource.String.RewardsPoints), profile.rewards);
            //edit.PutInt(GetString(Resource.String.DistributePoints), profile.distribute);
            //edit.PutBoolean(GetString(Resource.String.EmailNotification), profile.emailNotificaion);
            edit.PutString(GetString(Resource.String.CurrentAccessToken), MeritMoneyBrain.CurrentAccessToken);
            edit.Apply();

            ProfileDatabase db = new ProfileDatabase(GetString(Resource.String.ProfileDBFilename));
            db.createDatabase();
            db.Insert(profile);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            //throw new NotImplementedException();
        }

        protected void SetGoogleSingInButtonText(SignInButton signInButton, String buttonText)
        {
            // Find the TextView that is inside of the SignInButton and set its text
            for (int i = 0; i < signInButton.ChildCount; i++)
            {
                View v = signInButton.GetChildAt(i);

                if (v is TextView) {
                TextView tv = (TextView)v;
                tv.Text = buttonText;
                return;
            }
        }
    }
}
}