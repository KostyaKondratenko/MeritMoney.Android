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
using Android.Util;
using System.Threading.Tasks;
using System.Threading;

namespace Merit_Money
{
    [Activity(MainLauncher = true, NoHistory = true, Icon = "@drawable/cloud")]
    public class SplashScreenActivity : BaseBottomBarActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SplashScreen);

            new LoadProfile(this, new ProgressDialog(this)).Execute();
        }

        public override void OnBackPressed() { }

        private class LoadProfile : AsyncTask<Java.Lang.Void, Java.Lang.Void, Task<Java.Lang.Void>>
        {
            private ProgressDialog dialog;
            private SplashScreenActivity context;

            public LoadProfile(SplashScreenActivity activity, ProgressDialog dialog)
            {
                this.context = activity;
                this.dialog = dialog;
            }

            protected override async Task<Java.Lang.Void> RunInBackground(params Java.Lang.Void[] @params)
            {
                ISharedPreferences info = Application.Context.GetSharedPreferences(context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                bool LoggedIn = info.GetBoolean(context.GetString(Resource.String.LogIn), false);
                MeritMoneyBrain.CurrentAccessToken = info.GetString(context.GetString(Resource.String.CurrentAccessToken), String.Empty);

                if (context.NetworkStatus.State != NetworkState.Disconnected)
                {
                    if (LoggedIn)
                    {
                        Profile p = await MeritMoneyBrain.GetProfile();
                        ProfileDatabase db = new ProfileDatabase(context.GetString(Resource.String.ProfileDBFilename));
                        db.Update(p);
                    }
                    context.StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                }
                else
                {
                    await Task.Delay(500);
                    context.StartActivity(new Intent(Application.Context, typeof(NoInternetActivity)));
                }

                return null;
            }

            protected override void OnPreExecute()
            {
                base.OnPreExecute();
                dialog = ProgressDialog.Show(context, "", "Retrieving pofile data");
            }

            protected override void OnPostExecute(Task<Java.Lang.Void> result)
            {
                base.OnPostExecute(result);
                dialog.Dismiss();
                context.Finish();
            }
        }
    }
}