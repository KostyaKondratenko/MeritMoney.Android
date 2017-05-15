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
using Java.Lang;
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

        private HistoryList HistoryList;
        private HistoryType type;

        private RecyclerView HistoryView;
        private RecyclerView.Adapter RecyclerViewAdapter;

        private int Offset = 0; 
        private const int BatchSize = 10;

        public HistoryFragment(HistoryType type, NetworkStatusMonitor Network)
        {
            this.type = type;
            this.Network = Network;

            HistoryList = new HistoryList(type);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HistoryFragment, container, false);

            Refresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.history_swipe_refresh_layout);
            HistoryView = view.FindViewById<RecyclerView>(Resource.Id.HistoryList);
            LoadingPanel = view.FindViewById<View>(Resource.Id.loadingPanel);
            NoInternetText = view.FindViewById<TextView>(Resource.Id.NoInternetText);

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
                HistoryList = await MeritMoneyBrain.GetHistory(Offset, BatchSize, type);
                LoadingPanel.Visibility = ViewStates.Gone;
            }
            else
            {
                LoadingPanel.Visibility = ViewStates.Invisible;
                NoInternetText.Visibility = ViewStates.Visible;
            }

            var RecyclerViewManager = new LinearLayoutManager(this.Context);
            HistoryView.SetLayoutManager(RecyclerViewManager);
            RecyclerViewAdapter = new HistoryAdapter(HistoryList, this.Context);
            HistoryView.SetAdapter(RecyclerViewAdapter);

            foreach (HistoryListItem item in HistoryList)
                new CacheHistoryListItemImage(RecyclerViewAdapter, Application.Context).Execute(item);
        }

        private async void History_Refresh(object sender, EventArgs e)
        {
            if (Network.State != NetworkState.Disconnected)
            {
                Offset = 0;

                HistoryList = await MeritMoneyBrain.GetHistory(Offset, BatchSize, type);

                RecyclerViewAdapter = new HistoryAdapter(HistoryList, this.Context);
                HistoryView.SetAdapter(RecyclerViewAdapter);

                foreach (HistoryListItem item in HistoryList)
                    new CacheHistoryListItemImage(RecyclerViewAdapter, Application.Context).Execute(item);
            }
            else
            {
                Toast.MakeText(this.Context, "There is no Internet connection.", ToastLength.Short).Show();
            }

            Refresh.Refreshing = false;
        }
    }
}