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
using Android.Support.V7.Widget;

namespace Merit_Money
{
    [Activity(Label = "HistoryActivity")]
    public class HistoryActivity : BaseBottomBarActivity
    {
        private SupportToolBar MainToolbar;
        private SupportTabLayout TabLayout;
        private SupportViewPager ViewPager;
        private ViewPagerAdapter ViewPagerAdapter;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FrameLayout MainLayout = new FrameLayout(this);
            SetContentView(MainLayout);
            base.CombineWith(MainLayout, Resource.Layout.History, ActivityIs.History);

            MainToolbar = FindViewById<SupportToolBar>(Resource.Id.toolbar);
            TabLayout = FindViewById<SupportTabLayout>(Resource.Id.tabLayout);
            ViewPager = FindViewById<SupportViewPager>(Resource.Id.viewPager);

            MainToolbar.Title = "History";

            ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Loading list of users, please wait.", true);

            if (NetworkStatus.State != NetworkState.Disconnected)
            {
                UsersDatabase db = new UsersDatabase();

                ISharedPreferences info = Application.Context.GetSharedPreferences(Application.Context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                String Date = info.GetString(Application.Context.GetString(Resource.String.ModifyDate), String.Empty);

                List<UserListItem> tmp = await MeritMoneyBrain.GetListOfUsers(modifyAfter: Date);
                if (await db.IsExist())
                    await db.Merge(tmp);
                else
                {
                    await db.CreateDatabase();
                    await db.Insert(tmp);
                }
            }

            progressDialog.Dismiss();

            ViewPagerAdapter = new ViewPagerAdapter(SupportFragmentManager);
            ViewPagerAdapter.AddFragments(new HistoryFragment(HistoryType.Personal,
                NetworkStatus),
                new Java.Lang.String("Personal"));
            ViewPagerAdapter.AddFragments(new HistoryFragment(HistoryType.Company,
                NetworkStatus),
                new Java.Lang.String("Company"));
            ViewPager.Adapter = ViewPagerAdapter;

            TabLayout.SetupWithViewPager(ViewPager);

            var onScrollListener = new XamarinRecyclerViewOnScrollListener(new LinearLayoutManager(this));

        }
    }

    public class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);
        public event LoadMoreEventHandler LoadMoreEvent;

        private LinearLayoutManager LayoutManager;

        public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager)
        {
            LayoutManager = layoutManager;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            var visibleItemCount = recyclerView.ChildCount;
            var totalItemCount = recyclerView.GetAdapter().ItemCount;
            var pastVisiblesItems = LayoutManager.FindFirstVisibleItemPosition();

            if ((visibleItemCount + pastVisiblesItems) >= totalItemCount)
            {
                LoadMoreEvent(this, null);
            }
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

    public class HistoryAdapter : RecyclerView.Adapter
    {
        private HistoryList History;
        private Context context;

        public HistoryAdapter(HistoryList history, Context context)
        {
            this.History = history;
            this.context = context;
        }

        public class HistoryViewHolder : RecyclerView.ViewHolder
        {
            public View MainView { get; set; }
            public TextView message { get; set; }
            public TextView comment { get; set; }
            public TextView date { get; set; }
            public CircularImageView Avatar { get; set; }
            public ImageView Indicator { get; set; }
            public HistoryList History;
            public Context context;

            public HistoryViewHolder(View view, Context context, HistoryList history) : base(view)
            {
                MainView = view;
                this.History = history;
                this.context = context;
            }
        }

        public override int ItemCount
        {
            get { return History.Count(); }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            HistoryViewHolder Holder = holder as HistoryViewHolder;
            Holder.Avatar.SetImageBitmap(History[position].image);
            Holder.comment.Text = History[position].comment;
            Holder.message.Text = OrganizeMessageString(History[position]);
            Holder.date.Text = FromUnixTime(Convert.ToInt64(History[position].date));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View item = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_list_item, parent, false);

            TextView itemDate = item.FindViewById<TextView>(Resource.Id.historyDate);
            TextView itemMessage = item.FindViewById<TextView>(Resource.Id.historyName);
            TextView itemComment = item.FindViewById<TextView>(Resource.Id.historyReason);
            CircularImageView itemAvatar = item.FindViewById<CircularImageView>(Resource.Id.searchAvatar);
            ImageView itemIndicator = item.FindViewById<ImageView>(Resource.Id.history_indicator);

            itemIndicator.Visibility = ViewStates.Invisible;

            HistoryViewHolder view = new HistoryViewHolder(item, context, History)
            {
                date = itemDate,
                message = itemMessage,
                Avatar = itemAvatar,
                Indicator = itemIndicator,
                comment = itemComment
            };
            return view;
        }

        private String OrganizeMessageString(HistoryListItem value)
        {
            String result = String.Empty;

            if (value.message.Contains("sent"))
            {
                result = DefineSenderName(value.fromUserID) + " " + value.message;
            }
            else
            {
                result = value.message;
            }

            return result;
        }

        private String DefineSenderName(String ID)
        {
            UsersDatabase db = new UsersDatabase();
            return db.GetUserByID(ID).name;
        }

        private String FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime currentTime = DateTime.Now;

            epoch = epoch.AddSeconds(unixTime);

            if (currentTime.ToString("d.MM.yy") == epoch.ToString("d.MM.yy"))
            {
                return "Today";
            }

            if(currentTime.AddDays(-1).ToString("d.MM.yy") == epoch.ToString("d.MM.yy"))
            {
                return "Yesterday";
            }

            return epoch.ToString("d.MM.yy");
        }
    }
}