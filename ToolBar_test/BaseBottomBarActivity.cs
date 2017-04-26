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
using SupportBottomBar = Android.Support.Design.Widget.BottomNavigationView;
using Android.Views.InputMethods;
using Android.Graphics;
using Android.Net;
using System.IO;

namespace Merit_Money
{
    [Activity(Label = "BaseBottomBarActivity")]
    public class BaseBottomBarActivity : AppCompatActivity
    {
        private SupportBottomBar BottomBar;
        private const int BottomHeightInDps = 56;
        protected bool DoubleClickedToExit = false;
        protected NetworkStatusMonitor NetworkStatus = new NetworkStatusMonitor();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            NetworkStatus.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            NetworkStatus.Stop();
        }

        protected int ConvertDpToPx(int padding_in_dp)
        {
            float scale = Resources.DisplayMetrics.Density;
            return (int)(padding_in_dp * scale + 0.5f);
        }

        private int GetBottomHeightInPx()
        {
            return ConvertDpToPx(BottomHeightInDps);
        }

        protected void CombineWith(FrameLayout MainLayout,
            int mainLayoutId, ActivityIs curScreen)
        {

            LayoutInflater inflate = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);

            RelativeLayout main = (RelativeLayout)LayoutInflater.Inflate(
               mainLayoutId, null);
            RelativeLayout bottom = (RelativeLayout)LayoutInflater.Inflate(
                Resource.Layout.bottom_bar, null);

            main.SetPadding(0, 0, 0, ConvertDpToPx(BottomHeightInDps));

            MainLayout.AddView(main, RelativeLayout.LayoutParams.MatchParent,
              RelativeLayout.LayoutParams.MatchParent);
            MainLayout.AddView(bottom, RelativeLayout.LayoutParams.MatchParent,
              RelativeLayout.LayoutParams.MatchParent);

            BottomBarClickInit(curScreen);
        }

        public override void OnBackPressed()
        {
            if (DoubleClickedToExit)
            {
                base.OnBackPressed();
                Finish();
                return;
            }

            this.DoubleClickedToExit = true;
            Toast.MakeText(this, "Press Back to Exit", ToastLength.Short).Show();

            new Handler().PostDelayed(() =>
            {
                DoubleClickedToExit = false;
            }, 3000);
        }

        private void BottomBarClickInit(ActivityIs curScreen)
        {
            BottomBar = FindViewById<SupportBottomBar>(Resource.Id.bottom_navigation_bar);
            BottomBar.Menu.GetItem((int)curScreen).SetChecked(true);
            BottomBar.NavigationItemSelected += BottomBar_Click;
        }

        private void BottomBar_Click(object sender, SupportBottomBar.NavigationItemSelectedEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.bottom_home:
                    Intent homeIntent = new Intent(this, typeof(MainActivity));
                    homeIntent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.NoAnimation);
                    this.StartActivity(homeIntent);
                    Finish();
                    OverridePendingTransition(0, 0);
                    break;
                case Resource.Id.bottom_history:
                    Intent historyIntent = new Intent(this, typeof(HistoryActivity));
                    historyIntent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.NoAnimation);
                    this.StartActivity(historyIntent);
                    Finish();
                    OverridePendingTransition(0, 0);
                    break;
                case Resource.Id.bottom_profile:
                    Intent profileIntent = new Intent(this, typeof(ProfileActivity));
                    profileIntent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.NoAnimation);
                    this.StartActivity(profileIntent);
                    Finish();
                    OverridePendingTransition(0, 0);
                    break;
            }
        }

        protected void ShowKeyboard(EditText editText)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(editText, ShowFlags.Forced);
            imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
            editText.SetSelection(editText.Text.Length);
        }

        protected void HideKeyboard(EditText editText)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(editText.WindowToken, HideSoftInputFlags.None);
        }

        protected class SetImageFromUrl : AsyncTask<String, Java.Lang.Void, Bitmap>
        {
            CircularImageView image;

            public SetImageFromUrl(CircularImageView image)
            {
                this.image = image;
            }

            protected override Android.Graphics.Bitmap RunInBackground(params String[] @params)
            {
                String imageUrl = @params[0];
                Bitmap imageBitmap = MeritMoneyBrain.GetImageBitmapFromUrl(imageUrl);

                return imageBitmap;
            }

            protected override void OnPostExecute(Bitmap result)
            {
                if (result != null)
                {
                    image.SetImageBitmap(result);
                }
                else
                {
                    image.SetImageResource(Resource.Drawable.ic_noavatar);
                }
                
                base.OnPostExecute(result);
            }
        }

        protected class LoadAndSaveImage : AsyncTask<String, Java.Lang.Void, Bitmap>
        {
            CircularImageView image;

            public LoadAndSaveImage(CircularImageView image)
            {
                this.image = image;
            }

            protected override Android.Graphics.Bitmap RunInBackground(params String[] @params)
            {
                String imageUrl = @params[0];
                String userId = @params[1];

                if(imageUrl == null)
                {
                    return null;
                }

                Bitmap imageBitmap = MeritMoneyBrain.GetImageBitmapFromUrl(imageUrl);

                var sdCardPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                //var sdCardPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                var filePath = System.IO.Path.Combine(sdCardPath, userId);
                using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    imageBitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);

                }
                return imageBitmap;
            }

            protected override void OnPostExecute(Bitmap result)
            {
                if (result != null)
                {
                    image.SetImageBitmap(result);
                }
                else
                {
                    image.SetImageResource(Resource.Drawable.ic_noavatar);
                }

                base.OnPostExecute(result);
            }
        }
    }
}