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

        private History PersonalHistoryList;
        private History CompanyHistoryList;

        private int Offset = 0;
        private const int BatchSize = 5;

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

            UsersDatabase db = new UsersDatabase(GetString(Resource.String.UsersDBFilename));
            if (db.GetUsers() == null)
            {
                db.createDatabase();
                db.Insert(await MeritMoneyBrain.GetListOfUsers());
            }

            progressDialog.SetMessage("Loading history, please wait.");

            PersonalHistoryList = await MeritMoneyBrain.GetHistory(Offset, BatchSize, HistoryType.Personal);
            CompanyHistoryList = await MeritMoneyBrain.GetHistory(Offset, BatchSize, HistoryType.Company);

            ViewPagerAdapter = new ViewPagerAdapter(SupportFragmentManager);
            ViewPagerAdapter.AddFragments(new HistoryFragment(PersonalHistoryList), new Java.Lang.String("Personal"));
            ViewPagerAdapter.AddFragments(new HistoryFragment(CompanyHistoryList), new Java.Lang.String("Company"));
            ViewPager.Adapter = ViewPagerAdapter;

            TabLayout.SetupWithViewPager(ViewPager);

            progressDialog.Dismiss();

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

    public class PersonalHistoryAdapter : RecyclerView.Adapter
    {
        private History PersonalHistory;
        private Context context;

        public PersonalHistoryAdapter(History history, Context context)
        {
            this.PersonalHistory = history;
            this.context = context;
        }

        public class HistoryViewHolder : RecyclerView.ViewHolder
        {
            public View MainView { get; set; }
            public TextView message { get; set; }
            public TextView comment { get; set; }
            public TextView date { get; set; }
            public CircularImageView Avatar { get; set; }
            public CircularImageView Indicator { get; set; }
            public History PersonalHistory;
            public Context context;

            public HistoryViewHolder(View view, Context context, History history) : base(view)
            {
                MainView = view;
                this.PersonalHistory = history;
                this.context = context;
            }
        }

        public override int ItemCount
        {
            get { return PersonalHistory.Count(); }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            HistoryViewHolder Holder = holder as HistoryViewHolder;
            //Holder.Avatar.SetImageBitmap(PersonalHistory[position].);
            Holder.comment.Text = PersonalHistory[position].comment;
            Holder.message.Text = OrganizeMessageString(PersonalHistory[position]);
            //Holder.date.Text = PersonalHistory[position].date.ToString();
            Holder.date.Text = FromUnixTime(Convert.ToInt64(PersonalHistory[position].date)).ToString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View item = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_list_item, parent, false);

            TextView itemDate = item.FindViewById<TextView>(Resource.Id.historyDate);
            TextView itemMessage = item.FindViewById<TextView>(Resource.Id.historyName);
            TextView itemComment = item.FindViewById<TextView>(Resource.Id.historyReason);
            //AVATAR
            CircularImageView itemAvatar = item.FindViewById<CircularImageView>(Resource.Id.searchAvatar);
            CircularImageView itemIndicator = item.FindViewById<CircularImageView>(Resource.Id.history_indicator);

            HistoryViewHolder view = new HistoryViewHolder(item, context, PersonalHistory)
            {
                date = itemDate,
                message = itemMessage,
                Avatar = itemAvatar,
                Indicator = itemIndicator,
                comment = itemComment
            };
            return view;
        }

        private String OrganizeMessageString(HistoryObject value)
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
            UsersDatabase db = new UsersDatabase(context.GetString(Resource.String.UsersDBFilename));
            return db.GetUserNameByID(ID);
        }

        private DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
    }
}