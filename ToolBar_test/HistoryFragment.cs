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
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using System.Threading.Tasks;

namespace Merit_Money
{
    [Activity(Label = "HistoryFragment")]
    public class HistoryFragment : Android.Support.V4.App.Fragment
    {
        private SwipeRefreshLayout Refresh;
        private View LoadingPanel;
        private TextView NoInternetText;
        private NetworkStatusMonitor Network;
        private TextView Initials;

        private HistoryList historyList;
        private HistoryType type;
        private String LastHistoryItemDate;

        private RecyclerView HistoryView;
        private RecyclerView.Adapter RecyclerViewAdapter;

        private int Offset = 0;
        private const int BatchSize = 10;
        private ScrollListener mScrollListener;
        private LinearLayoutManager RecyclerViewManager;

        public HistoryFragment(HistoryType type,String LastHistoryItemDate, NetworkStatusMonitor Network)
        {
            this.type = type;
            this.Network = Network;
            this.LastHistoryItemDate = LastHistoryItemDate;

            historyList = new HistoryList(type);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HistoryFragment, container, false);

            Refresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.history_swipe_refresh_layout);
            HistoryView = view.FindViewById<RecyclerView>(Resource.Id.HistoryList);
            LoadingPanel = view.FindViewById<View>(Resource.Id.loadingPanel);
            NoInternetText = view.FindViewById<TextView>(Resource.Id.NoInternetText);
            Initials = view.FindViewById<TextView>(Resource.Id.Initials);

            NoInternetText.Visibility = ViewStates.Invisible;

            Refresh.Refresh += History_Refresh;

            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Refresh.Refresh -= History_Refresh;
            GC.Collect();
        }

        public override async void OnResume()
        {
            base.OnResume();

            if (Network.State != NetworkState.Disconnected)
            {
                historyList = await MeritMoneyBrain.GetHistory(Offset, BatchSize, type);
                LoadingPanel.Visibility = ViewStates.Gone;
            }
            else
            {
                LoadingPanel.Visibility = ViewStates.Invisible;
                NoInternetText.Visibility = ViewStates.Visible;
            }

            RecyclerViewManager = new LinearLayoutManager(this.Context);
            HistoryView.SetLayoutManager(RecyclerViewManager);
            RecyclerViewAdapter = new HistoryAdapter(historyList, this.Context, LastHistoryItemDate);
            HistoryView.SetAdapter(RecyclerViewAdapter);

            mScrollListener = new ScrollListener(RecyclerViewManager, historyList, Initials, type);
            HistoryView.AddOnScrollListener(mScrollListener);

            foreach (HistoryListItem item in historyList)
                new CacheListItemImage(RecyclerViewAdapter,Initials, Application.Context).Execute(item);
        }

        private async void History_Refresh(object sender, EventArgs e)
        {
            if (Network.State != NetworkState.Disconnected)
            {
                Offset = 0;

                historyList = await MeritMoneyBrain.GetHistory(Offset, BatchSize, type);

                ISharedPreferences info = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                LastHistoryItemDate = info.GetString(GetString(Resource.String.HistoryLoadedDate), "0");

                RecyclerViewAdapter = new HistoryAdapter(historyList, this.Context, LastHistoryItemDate);
                HistoryView.SetAdapter(RecyclerViewAdapter);//SWAP?
                mScrollListener = new ScrollListener(RecyclerViewManager, historyList, Initials, type);
                HistoryView.AddOnScrollListener(mScrollListener);

                foreach (HistoryListItem item in historyList)
                    new CacheListItemImage(RecyclerViewAdapter, Initials, Application.Context).Execute(item);
                
            }
            else
            {
                Toast.MakeText(this.Context, GetString(Resource.String.NoInternet), ToastLength.Short).Show();
            }

            Refresh.Refreshing = false;
        }

        public class ScrollListener : EndlessRecyclerViewScrollListener
        {
            private HistoryList history;
            private HistoryType type;
            private TextView Initials;

            public ScrollListener(LinearLayoutManager manager, HistoryList list,TextView Initials, HistoryType type)
                : base(manager)
            {
                this.history = list;
                this.type = type;
                this.Initials = Initials;
            }

            public override async void onLoadMore(int page, RecyclerView view)
            {
                if (history.hasMore)
                {
                    HistoryList listItem = await MeritMoneyBrain.GetHistory(page * BatchSize, BatchSize, type);

                    UsersDatabase db = new UsersDatabase();
                    foreach (HistoryListItem item in listItem)
                        new CacheListItemImage(view.GetAdapter(), Initials, Application.Context).Execute(item);
                    
                    var previousPosition = history.Count();
                    var itemsAdded = listItem.Count();

                    history.AddList(listItem);
                    history.hasMore = listItem.hasMore;

                    view.GetAdapter().NotifyItemRangeInserted(previousPosition, itemsAdded);
                }
            }
        }
    }
}