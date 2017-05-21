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
    public class LogInActivity : BaseBottomBarActivity, GoogleApiClient.IOnConnectionFailedListener
    {
        private SignInButton LogIn;
        private GoogleApiClient GoogleClient;

        private readonly int SIGN_IN = 7;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LogInWithEmail);

            LogIn = FindViewById<SignInButton>(Resource.Id.LogInButton);
            SetGoogleSingInButtonText(LogIn, "Log in with Google");

            LogIn.Click += LogIn_Clicked;

            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestEmail()
                    .RequestIdToken(GetString(Resource.String.ServerClientID))
                    .Build();


            GoogleClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();
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
                ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Retrieving profile data", true);
                GoogleSignInAccount acct = result.SignInAccount;
                await MeritMoneyBrain.SingInWithGoogle(acct.IdToken);
                Profile profile = await MeritMoneyBrain.GetProfile();
                Intent returnIntent = new Intent();
                returnIntent.PutExtra(GetString(Resource.String.LogIn), true);
                SetResult(Result.Ok, returnIntent);
                await new SaveData().Execute(profile).GetAsync();
                progressDialog.Dismiss();
                Finish();
                //updateUI(true);
            }
            else
            {
                // Signed out, show unauthenticated UI.
                //updateUI(false);
            }
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

                if (v is TextView)
                {
                    TextView tv = (TextView)v;
                    tv.Text = buttonText;
                    return;
                }
            }
        }

        public override void OnBackPressed() { }

        private class SaveData : AsyncTask<Profile, Java.Lang.Void, Java.Lang.Void>
        {
            protected override Java.Lang.Void RunInBackground(params Profile[] @params)
            {
                Profile profile = @params[0];
                ISharedPreferences info = Application.Context.GetSharedPreferences(Application.Context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                ISharedPreferencesEditor edit = info.Edit();
                edit.PutString(Application.Context.GetString(Resource.String.CurrentAccessToken), MeritMoneyBrain.CurrentAccessToken);
                edit.Apply();

                profile.AvatarIsDefault = OperationWithBitmap.isDefault(profile.imageUri);

                ProfileDatabase db = new ProfileDatabase();
                db.CreateDatabase();
                db.Insert(profile);

                return null;
            }
        }
    }
}