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
    public abstract class EndlessRecyclerViewScrollListener : RecyclerView.OnScrollListener
    {
        private int visibleThreshold = 5;
        private int currentPage = 0;
        private int previousTotalItemCount = 0;
        private bool loading = true;
        private int startingPageIndex = 0;

        private RecyclerView.LayoutManager mLayoutManager;

        public EndlessRecyclerViewScrollListener(LinearLayoutManager layoutManager)
        {
            this.mLayoutManager = layoutManager;
        }

        public int GetLastVisibleItem(int[] lastVisibleItemPositions)
        {
            int maxSize = 0;
            for (int i = 0; i < lastVisibleItemPositions.Length; i++)
            {
                if (i == 0)
                {
                    maxSize = lastVisibleItemPositions[i];
                }
                else if (lastVisibleItemPositions[i] > maxSize)
                {
                    maxSize = lastVisibleItemPositions[i];
                }
            }
            return maxSize;
        }


        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            int lastVisibleItemPosition = 0;
            int totalItemCount = mLayoutManager.ItemCount;

            lastVisibleItemPosition = ((LinearLayoutManager)mLayoutManager).FindLastVisibleItemPosition();

            if (totalItemCount < previousTotalItemCount)
            {
                this.currentPage = this.startingPageIndex;
                this.previousTotalItemCount = totalItemCount;

                if (totalItemCount == 0)
                {
                    this.loading = true;
                }
            }

            if (loading && (totalItemCount > previousTotalItemCount))
            {
                loading = false;
                previousTotalItemCount = totalItemCount;
            }

            if (!loading && (lastVisibleItemPosition + visibleThreshold) > totalItemCount)
            {
                currentPage++;
                onLoadMore(currentPage, recyclerView);
                loading = true;
            }
        }

        public void resetState()
        {
            this.currentPage = this.startingPageIndex;
            this.previousTotalItemCount = 0;
            this.loading = true;
        }

        public abstract void onLoadMore(int page, RecyclerView view);
    }
}