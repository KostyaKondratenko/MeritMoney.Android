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
using SupportSearchView = Android.Support.V7.Widget.SearchView;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;
using SupportRecyclerView = Android.Support.V7.Widget.RecyclerView;
using Android.Support.V7.Widget;
using Android.Text;
using System.Threading;

namespace Merit_Money
{
    [Activity(Label = "SearchPersonActivity")]
    public class SearchPersonActivity : AppCompatActivity
    {
        private SupportToolBar ToolBar;
        private EditText ToolBarSearchView;
        private SupportRecyclerView SearchUserView;
        private SupportRecyclerView.LayoutManager RecyclerViewManager;
        private SupportRecyclerView.Adapter RecyclerViewAdapter;
        private List<SingleUser> SearchUsersList;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SearchPerson);
            ToolBar = FindViewById<SupportToolBar>(Resource.Id.search_toolbar);
            SearchUserView = FindViewById<SupportRecyclerView>(Resource.Id.searchUserList);

            SetSupportActionBar(ToolBar);

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            ToolBarSearchView = FindViewById<EditText>(Resource.Id.search_view);

            ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Loading. Please wait...", true);
            SearchUsersList = await MeritMoneyBrain.GetListOfUsers();
            progressDialog.Dismiss();

            Thread thread = new Thread(() =>
            {
                foreach (SingleUser user in SearchUsersList)
                {
                    user.image = MeritMoneyBrain.GetImageBitmapFromUrl(user.url);
                }
            });
            thread.Start();

            RecyclerViewManager = new LinearLayoutManager(this);
            SearchUserView.SetLayoutManager(RecyclerViewManager);
            RecyclerViewAdapter = new Users(SearchUsersList);
            SearchUserView.SetAdapter(RecyclerViewAdapter);

            ToolBarSearchView.TextChanged += ToolBarSearchView_TextChanged;
            ToolBar.MenuItemClick += Toolbar_MenuItemClick;
        }

        private void ToolBarSearchView_TextChanged(object sender, TextChangedEventArgs e)
        {
            String text = e.Text.ToString().ToLower();
            List<SingleUser> newList = new List<SingleUser>();

            foreach (SingleUser user in SearchUsersList)
            {
                String name = user.name.ToLower();
                String email = user.email.ToLower();
                if (name.Contains(text) || email.Contains(text))
                {
                    newList.Add(user);
                }
            }
            RecyclerViewAdapter = new Users(newList);
            SearchUserView.SetAdapter(RecyclerViewAdapter);
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

    public class Users : RecyclerView.Adapter
    {
        private List<SingleUser> MeritMoneyUsers;

        public Users(List<SingleUser> users)
        {
            MeritMoneyUsers = users;
        }

        public class ListView : RecyclerView.ViewHolder
        {
            public View MainView { get; set; }
            public TextView Name { get; set; }
            public TextView Email { get; set; }
            public CircularImageView Avatar { get; set; }

            public ListView(View view) : base(view)
            {
                MainView = view;
            }
        }

        public override int ItemCount
        {
            get { return MeritMoneyUsers.Count; }
        }

        public override void OnBindViewHolder(SupportRecyclerView.ViewHolder holder, int position)
        {
            ListView Holder = holder as ListView;
            Holder.Name.Text = MeritMoneyUsers[position].name;
            Holder.Email.Text = MeritMoneyUsers[position].email;
            Holder.Avatar.SetImageBitmap(MeritMoneyUsers[position].image);
        }

        public override SupportRecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View item = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.search_list_item, parent, false);

            TextView itemName = item.FindViewById<TextView>(Resource.Id.searchName);
            TextView itemEmail = item.FindViewById<TextView>(Resource.Id.searchEmail);
            CircularImageView itemAvatar = item.FindViewById<CircularImageView>(Resource.Id.searchAvatar);

            ListView view = new ListView(item) { Name = itemName, Email = itemEmail, Avatar = itemAvatar };
            return view;
        }

    }
}