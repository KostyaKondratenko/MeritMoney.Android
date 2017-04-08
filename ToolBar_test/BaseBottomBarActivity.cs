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
using ToolBar_test;
using SupportBottomBar = Android.Support.Design.Widget.BottomNavigationView;

namespace Merit_Money
{
    [Activity(Label = "BaseBottomBarActivity")]
    public class BaseBottomBarActivity : AppCompatActivity
    {
        private SupportBottomBar BottomBar;
        private const int BottomHeightInDps = 56;
        protected bool DoubleClickedToExit = false;

        protected int ConvertDpToPx(int padding_in_dp)
        {
            float scale = Resources.DisplayMetrics.Density;
            return (int)(padding_in_dp * scale + 0.5f);
        }

        protected Android.Graphics.Bitmap GetImageBitmapFromUrl(string url)
        {
            Android.Graphics.Bitmap imageBitmap = null;

            using (var webClient = new System.Net.WebClient())
            {
                if (url != String.Empty)
                {
                    try
                    {
                        var imageBytes = webClient.DownloadData(url);
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            imageBitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        }
                    }
                    catch(System.Net.WebException)
                    {
                           return null;   
                    }
                    
                }
            }

            return imageBitmap;
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
                    Toast.MakeText(this, "Bottom Bar Action: " + e.Item.TitleFormatted, ToastLength.Short).Show();
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
    }
}