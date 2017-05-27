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

        private List<UserListItem> users;
        private HistoryList historyList;
        private HistoryType type;
        private String LastHistoryItemDate;

        private RecyclerView HistoryView;
        private HistoryAdapter RecyclerViewAdapter;

        private int Offset = 0;
        private const int BatchSize = 10;
        private ScrollListener mScrollListener;
        private LinearLayoutManager RecyclerViewManager;

        public HistoryFragment(HistoryType type,String LastHistoryItemDate, 
            NetworkStatusMonitor Network,
            List<UserListItem> users)
        {
            this.type = type;
            this.Network = Network;
            this.LastHistoryItemDate = LastHistoryItemDate;
            this.users = users;

            historyList = new HistoryList(type);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HistoryFragment, container, false);

            Refresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.history_swipe_refresh_layout);
            HistoryView = view.FindViewById<RecyclerView>(Resource.Id.HistoryList);
            LoadingPanel = view.FindViewById<View>(Resource.Id.loadingPanel);
            NoInternetText = view.FindViewById<TextView>(Resource.Id.NoInternetText);

            NoInternetText.Visibility = ViewStates.Invisible;

            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Refresh.Refresh -= History_Refresh;
            GC.Collect();
        }

        public override void OnPause()
        {
            base.OnPause();
            Refresh.Refresh -= History_Refresh;
            RecyclerViewAdapter.Keep(BatchSize);
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

            Refresh.Refresh += History_Refresh;

            RecyclerViewManager = new LinearLayoutManager(this.Context);
            HistoryView.SetLayoutManager(RecyclerViewManager);
            RecyclerViewAdapter = new HistoryAdapter(historyList, this.Context, LastHistoryItemDate, users);
            HistoryView.SetAdapter(RecyclerViewAdapter);

            mScrollListener = new ScrollListener(RecyclerViewManager, historyList, type);
            HistoryView.AddOnScrollListener(mScrollListener);

            for (int i = 0;i <  historyList.Count();i++)
                new CacheListItemImage(RecyclerViewAdapter, i, Application.Context).Execute(historyList[i]);
        }

        private async void History_Refresh(object sender, EventArgs e)
        {
            if (Network.State != NetworkState.Disconnected)
            {
                Offset = 0;

                var historyList = await MeritMoneyBrain.GetHistory(Offset, BatchSize, type);

                ISharedPreferences info = Application.Context.GetSharedPreferences(Application.Context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                String Date = info.GetString(Application.Context.GetString(Resource.String.ModifyDate), String.Empty);
                LastHistoryItemDate = info.GetString(Application.Context.GetString(Resource.String.HistoryLoadedDate), "0");

                RecyclerViewAdapter.AddNewList(historyList, LastHistoryItemDate);
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

            public ScrollListener(LinearLayoutManager manager, HistoryList list, HistoryType type)
                : base(manager)
            {
                this.history = list;
                this.type = type;
            }

            public override async void onLoadMore(int page, RecyclerView view)
            {
                if (history.hasMore)
                {
                    HistoryList listItem = await MeritMoneyBrain.GetHistory(page * BatchSize, BatchSize, type);

                    var previousPosition = history.Count();
                    var itemsAdded = listItem.Count();

                    history.AddList(listItem);

                    view.GetAdapter().NotifyItemRangeInserted(previousPosition + 1, itemsAdded);

                    for (int i = previousPosition; i < previousPosition + itemsAdded; i++)
                        new CacheListItemImage(view.GetAdapter(), i, Application.Context).Execute(history[i]);
                }
            }
        }
    }
}