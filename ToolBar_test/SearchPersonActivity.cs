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
using SupportSearchView = Android.Support.V7.Widget.SearchView;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;

namespace ToolBar_test
{
    [Activity(Label = "SearchPersonActivity")]
    public class SearchPersonActivity : AppCompatActivity
    {
        SupportToolBar ToolBar;
        EditText ToolBarSearchView;
        ImageView SearchClearButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SearchPerson);
            ToolBar = FindViewById<SupportToolBar>(Resource.Id.search_toolbar);

            SetSupportActionBar(ToolBar);

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var sc = FindViewById(Resource.Id.search_container);
            ToolBarSearchView = FindViewById<EditText>(Resource.Id.search_view);
            SearchClearButton = FindViewById<ImageView>(Resource.Id.search_clear);

            IntPtr IntPtrTextViewClass = JNIEnv.FindClass(typeof(TextView));
            IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID(IntPtrTextViewClass, "mCursorDrawableRes", "I");
            JNIEnv.SetField(Control.Handle, mCursorDrawableResProperty, 0);
        }

        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.search_menu, menu);
        //    SupportSearchView searchView = FindViewById<SupportSearchView>(Resource.Id.action_search);
        //    SearchManager searchManager = (SearchManager)GetSystemService(SearchService);
        //    searchView.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
        //    return true;
        //}

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