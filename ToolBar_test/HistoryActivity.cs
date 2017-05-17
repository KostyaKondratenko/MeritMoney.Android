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
                {
                    await db.Update(tmp);
                }
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

        private const int VIEW_TYPE_ITEM = 0;
        private const int VIEW_TYPE_LOADING = 1;

        public HistoryAdapter(HistoryList history, Context context)
        {
            this.History = history;
            this.context = context;
        }

        private class LoadingViewHolder : RecyclerView.ViewHolder
        {
            public ProgressBar ProgressBar;

            public LoadingViewHolder(View view) : base(view)
            {
                ProgressBar = (ProgressBar)view.FindViewById(Resource.Id.progressBar2);
            }
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

                TextView itemDate = view.FindViewById<TextView>(Resource.Id.historyDate);
                TextView itemMessage = view.FindViewById<TextView>(Resource.Id.historyName);
                TextView itemComment = view.FindViewById<TextView>(Resource.Id.historyReason);
                CircularImageView itemAvatar = view.FindViewById<CircularImageView>(Resource.Id.searchAvatar);
                ImageView itemIndicator = view.FindViewById<ImageView>(Resource.Id.history_indicator);

                itemIndicator.Visibility = ViewStates.Invisible;

                date = itemDate;
                message = itemMessage;
                Avatar = itemAvatar;
                Indicator = itemIndicator;
                comment = itemComment;
            }
        }

        public override int ItemCount
        {
            get { return History == null ? 0 : History.Count() +1; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        { 
            if (holder is HistoryViewHolder) {
                HistoryViewHolder Holder = holder as HistoryViewHolder;
                Holder.Avatar.SetImageBitmap(History[position].image);
                Holder.comment.Text = History[position].comment;
                Holder.message.Text = OrganizeMessageString(History[position]);
                Holder.date.Text = FromUnixTime(Convert.ToInt64(History[position].date));
            } else if (holder is LoadingViewHolder) {
                LoadingViewHolder LoadingViewHolder = holder as LoadingViewHolder;
                if (History.hasMore)
                    LoadingViewHolder.ProgressBar.Visibility = ViewStates.Visible;
                else
                    LoadingViewHolder.ProgressBar.Visibility = ViewStates.Gone;
            }
        }

        public override int GetItemViewType(int position)
        {
            return History[position] == null ? VIEW_TYPE_LOADING : VIEW_TYPE_ITEM;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == VIEW_TYPE_ITEM)
            {
                View item = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_list_item, parent, false);
                return new HistoryViewHolder(item, context, History);
            }
            else if (viewType == VIEW_TYPE_LOADING)
            {
                View item = LayoutInflater.From(parent.Context).Inflate(Resource.Drawable.progress_bar, parent, false);
                return new LoadingViewHolder(item);
            }

            return null;
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