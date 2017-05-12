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
using SupportSearchView = Android.Support.V7.Widget.SearchView;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;
using SupportRecyclerView = Android.Support.V7.Widget.RecyclerView;
using Android.Support.V7.Widget;
using Android.Text;
using System.Threading;
using Android.Views.InputMethods;

namespace Merit_Money
{
    [Activity(Label = "SearchPersonActivity")]
    public class SearchPersonActivity : BaseBottomBarActivity, 
        Android.Support.V7.Widget.SearchView.IOnQueryTextListener
    {
        private SupportToolBar ToolBar;
        private SupportRecyclerView SearchUserView;
        private SupportRecyclerView.LayoutManager RecyclerViewManager;
        private UsersAdapter RecyclerViewAdapter;
        private List<UserListItem> SearchUsersList;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SearchPerson);
            ToolBar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            SearchUserView = FindViewById<SupportRecyclerView>(Resource.Id.searchUserList);

            SetSupportActionBar(ToolBar);
            SupportActionBar.Title = "Select Person";

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            UsersDatabase db = new UsersDatabase();

            ISharedPreferences info = Application.Context.GetSharedPreferences(Application.Context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            String Date = info.GetString(Application.Context.GetString(Resource.String.ModifyDate), String.Empty);

            ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Loading, please wait...", true);

            List<UserListItem> tmp = await MeritMoneyBrain.GetListOfUsers(modifyBefore: Date);
            if (await db.IsExist())
                await db.Merge(tmp);
            else
            {
                await db.CreateDatabase();
                await db.Insert(tmp);
            }

            SearchUsersList = await db.GetUsers();

            progressDialog.Dismiss();


            RecyclerViewManager = new LinearLayoutManager(this);
            SearchUserView.SetLayoutManager(RecyclerViewManager);
            RecyclerViewAdapter = new UsersAdapter(SearchUsersList, this);
            SearchUserView.SetAdapter(RecyclerViewAdapter);


            foreach (UserListItem user in SearchUsersList)
                new CacheUserListItemImage(RecyclerViewAdapter, Application.Context).Execute(user);

            ToolBar.MenuItemClick += ToolBar_MenuItemClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ToolBar.MenuItemClick -= ToolBar_MenuItemClick;
            GC.Collect();
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private async void ToolBar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
        {
            if (NetworkStatus.State != NetworkState.Disconnected)
            {
                switch (e.Item.ItemId)
                {
                    case Resource.Id.menu_refresh:
                        ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Loading, please wait", true);

                        SearchUsersList = await MeritMoneyBrain.GetListOfUsers(String.Empty);

                        UsersDatabase db = new UsersDatabase();
                        await db.Update(SearchUsersList);

                        RecyclerViewAdapter = new UsersAdapter(SearchUsersList, this);
                        SearchUserView.SetAdapter(RecyclerViewAdapter);

                        foreach (UserListItem user in SearchUsersList)
                            new CacheUserListItemImage(RecyclerViewAdapter, Application.Context).Execute(user);

                        progressDialog.Dismiss();
                        break;
                }
            }
            else
            {
                Toast.MakeText(this, "There is no Internet connection.", ToastLength.Short).Show();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.search_top_menu, menu);
            IMenuItem menuItem = menu.FindItem(Resource.Id.action_search);
            Android.Support.V7.Widget.SearchView searchView = (Android.Support.V7.Widget.SearchView)Android.Support.V4.View.MenuItemCompat.GetActionView(menuItem);
            searchView.SetOnQueryTextListener(this);
            return base.OnCreateOptionsMenu(menu);
        }

        bool SupportSearchView.IOnQueryTextListener.OnQueryTextChange(string newText)
        {
            String text = newText.ToLower();
            List<UserListItem> newList = new List<UserListItem>();

            foreach (UserListItem user in SearchUsersList)
            {
                String name = user.name.ToLower();
                String email = user.email.ToLower();
                if (name.Contains(text) || email.Contains(text))
                {
                    newList.Add(user);
                }
            }
            RecyclerViewAdapter = new UsersAdapter(newList, this);
            SearchUserView.SetAdapter(RecyclerViewAdapter);

            return true;
        }

        bool SupportSearchView.IOnQueryTextListener.OnQueryTextSubmit(string query)
        {
            return false;
        }

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

    public class UsersAdapter : RecyclerView.Adapter
    {
        private List<UserListItem> MeritMoneyUsers;
        private SearchPersonActivity activity;

        public UsersAdapter(List<UserListItem> users, SearchPersonActivity activity)
        {
            MeritMoneyUsers = users;
            this.activity = activity;
        }

        public class ListViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        { 
            public View MainView { get; set; }
            public TextView Name { get; set; }
            public TextView Email { get; set; }
            public CircularImageView Avatar { get; set; }
            public List<UserListItem> users;
            public SearchPersonActivity activity;

            public ListViewHolder(View view, SearchPersonActivity activity, List<UserListItem> users) : base(view)
            {
                MainView = view;
                this.users = users;
                this.activity = activity;
                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                int position = AdapterPosition;
                UserListItem user = users[position];
                Intent returnIntent = new Intent();
                v.Selected = true;
                returnIntent.PutExtra(Application.Context.GetString(Resource.String.ID), user.ID);
                returnIntent.PutExtra(Application.Context.GetString(Resource.String.UserName), user.name);
                activity.SetResult(Result.Ok, returnIntent);
                activity.Finish();
            }
        }

        public override int ItemCount
        {
            get { return MeritMoneyUsers.Count; }
        }

        public override void OnBindViewHolder(SupportRecyclerView.ViewHolder holder, int position)
        {
            ListViewHolder Holder = holder as ListViewHolder;
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

            ListViewHolder view = new ListViewHolder(item, activity, MeritMoneyUsers) { Name = itemName, Email = itemEmail, Avatar = itemAvatar };
            return view;
        }
    }
}