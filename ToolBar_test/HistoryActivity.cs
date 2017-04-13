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
using SupportToolBar = Android.Support.V7.Widget.Toolbar;
using SupportTabLayout = Android.Support.Design.Widget.TabLayout;
using SupportViewPager = Android.Support.V4.View.ViewPager;
using Android.Support.V4.App;

namespace Merit_Money
{
    [Activity(Label = "HistoryActivity")]
    public class HistoryActivity : BaseBottomBarActivity
    {
        SupportToolBar MainToolbar;
        SupportTabLayout TabLayout;
        SupportViewPager ViewPager;
        ViewPagerAdapter ViewPagerAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FrameLayout MainLayout = new FrameLayout(this);
            SetContentView(MainLayout);
            base.CombineWith(MainLayout, Resource.Layout.History, ActivityIs.History);

            MainToolbar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            TabLayout = FindViewById<SupportTabLayout>(Resource.Id.tabLayout);
            ViewPager = FindViewById<SupportViewPager>(Resource.Id.viewPager);

            ViewPagerAdapter = new ViewPagerAdapter(SupportFragmentManager);
            ViewPagerAdapter.AddFragments(new PersonalFragment(), new Java.Lang.String("Personal"));
            ViewPagerAdapter.AddFragments(new CompanyFragment(), new Java.Lang.String("Company"));
            ViewPager.Adapter = ViewPagerAdapter;

            TabLayout.SetupWithViewPager(ViewPager);

            MainToolbar.Title = "History";
        }
    }

    public class ViewPagerAdapter : FragmentPagerAdapter
    {
        List<Android.Support.V4.App.Fragment> fragments = new List<Android.Support.V4.App.Fragment>();
        List<Java.Lang.ICharSequence> tabTitles = new List<Java.Lang.ICharSequence>();

        public void AddFragments(Android.Support.V4.App.Fragment fragment, Java.Lang.ICharSequence title)
        {
            this.fragments.Add(fragment);
            this.tabTitles.Add(title);
        }

        public ViewPagerAdapter(Android.Support.V4.App.FragmentManager manager) : base(manager)
        {
        } 

        public override int Count
        {
            get { return fragments.Count; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return tabTitles[position];
        }
    }
}