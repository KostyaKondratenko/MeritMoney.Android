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
using Android.Graphics;

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

            List<UserListItem> tmp = await MeritMoneyBrain.GetListOfUsers(modifyAfter: Date);
            if (db.IsExist())
                db.Merge(tmp);
            else
            {
                db.CreateDatabase();
                db.Insert(tmp);
            }

            SearchUsersList = db.GetUsers();

            progressDialog.Dismiss();

            RecyclerViewManager = new LinearLayoutManager(this);
            SearchUserView.SetLayoutManager(RecyclerViewManager);
            RecyclerViewAdapter = new UsersAdapter(SearchUsersList, this);
            SearchUserView.SetAdapter(RecyclerViewAdapter);

            for(int i = 0; i < SearchUsersList.Count;i++)
                new CacheListItemImage(RecyclerViewAdapter, i, Application.Context).Execute(SearchUsersList[i]);

            //ToolBar.MenuItemClick += ToolBar_MenuItemClick;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //ToolBar.MenuItemClick -= ToolBar_MenuItemClick;
            GC.Collect();
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        //private async void ToolBar_MenuItemClick(object sender, SupportToolBar.MenuItemClickEventArgs e)
        //{
        //    if (NetworkStatus.State != NetworkState.Disconnected)
        //    {
        //        switch (e.Item.ItemId)
        //        {
        //            //case Resource.Id.menu_refresh:
        //            //    ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Loading, please wait", true);

        //            //    SearchUsersList = await MeritMoneyBrain.GetListOfUsers(String.Empty);

        //            //    UsersDatabase db = new UsersDatabase();
        //            //    db.Update(SearchUsersList);

        //            //    RecyclerViewAdapter.AddNewList(SearchUsersList);

        //            //    for (int i = 0; i < SearchUsersList.Count(); i++)
        //            //        new CacheListItemImage(RecyclerViewAdapter, i, Application.Context).Execute(SearchUsersList[i]);

        //            //    progressDialog.Dismiss();
        //            //    break;
        //        }
        //    }
        //    else
        //    {
        //        Toast.MakeText(this, GetString(Resource.String.NoInternet), ToastLength.Short).Show();
        //    }
        //}

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
        private String curUserId;
        private const int AvatarSize = 70;

        public UsersAdapter(List<UserListItem> users, SearchPersonActivity activity)
        {
            MeritMoneyUsers = users;
            this.activity = activity;

            ProfileDatabase db = new ProfileDatabase();
            curUserId = db.GetProfile().ID;
            for (int i = 0; i < MeritMoneyUsers.Count; i++)
                if (MeritMoneyUsers[i].ID == curUserId)
                    MeritMoneyUsers.RemoveAt(i);
        }

        public void AddList(List<UserListItem> list)
        {
            ProfileDatabase pdb = new ProfileDatabase();
            String curId = pdb.GetProfile().ID;

            for (int i = 0; i < list.Count; i++)
                if (curId == list[i].ID)
                    list.RemoveAt(i);

            int startingPos = MeritMoneyUsers.Count();
            MeritMoneyUsers.AddRange(list);
            NotifyItemRangeInserted(startingPos, list.Count());

            for (int i = startingPos; i < startingPos + list.Count(); i++)
                new CacheListItemImage(this, i, Application.Context).Execute(MeritMoneyUsers[i]);
        }

        public void AddNewList(List<UserListItem> list)
        {
            ProfileDatabase pdb = new ProfileDatabase();
            String curId = pdb.GetProfile().ID;

            for (int i = 0; i < list.Count; i++)
                if (curId == list[i].ID)
                    list.RemoveAt(i);

            MeritMoneyUsers.Clear();
            MeritMoneyUsers.AddRange(list);
            NotifyDataSetChanged();
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

            //if (MeritMoneyUsers[position].AvatarIsDefault)
            //{
            //Bitmap bitmap = Bitmap.CreateBitmap(AdditionalFunctions.ConvertDpToPx(AvatarSize),
            //AdditionalFunctions.ConvertDpToPx(AvatarSize), Bitmap.Config.Argb8888);

            //Bitmap avatar = AdditionalFunctions.DrawTextToBitmap(bitmap,
            //    AdditionalFunctions.DefineInitials(MeritMoneyUsers[position].name),
            //    AdditionalFunctions.ConvertDpToPx(AvatarSize / 3));

            //MeritMoneyUsers[position].image = avatar;
            //}

            Holder.Avatar.SetImageBitmap(MeritMoneyUsers[position].image);
        }

        public override SupportRecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View item = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.search_list_item, parent, false);

            TextView itemName = item.FindViewById<TextView>(Resource.Id.searchName);
            TextView itemEmail = item.FindViewById<TextView>(Resource.Id.searchEmail);
            CircularImageView itemAvatar = item.FindViewById<CircularImageView>(Resource.Id.searchAvatar);
            TextView itemInitials = item.FindViewById<TextView>(Resource.Id.Initials);

            ListViewHolder view = new ListViewHolder(item, activity, MeritMoneyUsers) { Name = itemName, Email = itemEmail, Avatar = itemAvatar};
            return view;
        }
    }
}