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

            ToolBar.MenuItemClick += Toolbar_MenuItemClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.search_top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private void Toolbar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.search_exit_menu:
                    ToolBarSearchView.Text = "";
                    break;
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    ToolBarSearchView.Text = "";
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}