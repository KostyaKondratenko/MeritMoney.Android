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

namespace Merit_Money
{
    [Activity(Label = "HistoryFragment")]
    public class HistoryFragment : Android.Support.V4.App.Fragment
    {
        History HistoryList;

        public HistoryFragment(History HistoryList)
        {
            this.HistoryList = HistoryList;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.HistoryFragment, container, false);

            var HistoryView = view.FindViewById<RecyclerView>(Resource.Id.HistoryList);
            var RecyclerViewManager = new LinearLayoutManager(this.Context);
            HistoryView.SetLayoutManager(RecyclerViewManager);
            var RecyclerViewAdapter = new PersonalHistoryAdapter(HistoryList, this.Context);
            HistoryView.SetAdapter(RecyclerViewAdapter);

            return view;
        }
    }
}