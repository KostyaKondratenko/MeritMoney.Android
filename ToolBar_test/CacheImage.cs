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
using Android.Graphics;
using System.IO;
using Android.Support.V7.Widget;

namespace Merit_Money
{
    public class CacheHistoryListItemImage : AsyncTask<HistoryListItem, Java.Lang.Void, HistoryListItem>
    {
        private RecyclerView.Adapter adapter;
        private Context context;

        public CacheHistoryListItemImage(RecyclerView.Adapter adapter, Context context)
        {
            this.adapter = adapter;
            this.context = context;
        }

        protected override HistoryListItem RunInBackground(params HistoryListItem[] @params)
        {
            HistoryListItem user = @params[0];
            try
            {
                byte[] data = OperationWithBitmap.Retrieve(context, user.ID);

                if (data == null)
                {
                    UsersDatabase db = new UsersDatabase();
                    user.image = OperationWithBitmap.GetFromUrl(db.GetUserByID(user.fromUserID).url);
                    var bitmapData = OperationWithBitmap.ConvertToByteArray(user.image);
                    OperationWithBitmap.Cache(context, bitmapData, user.ID);
                }
                else
                {
                    user.image = OperationWithBitmap.ConvertFromByteArray(data);
                }
            }
            catch (Exception e) { Console.Out.WriteLine(e.Message); }

            return user;
        }

        protected override void OnPostExecute(HistoryListItem result)
        {
            adapter.NotifyDataSetChanged();
            base.OnPostExecute(result);
        }

    }

    public class CacheUserListItemImage : AsyncTask<UserListItem, Java.Lang.Void, UserListItem>
    {
        private RecyclerView.Adapter adapter;
        private Context context;

        public CacheUserListItemImage(RecyclerView.Adapter adapter, Context context)
        {
            this.adapter = adapter;
            this.context = context;
        }

        protected override UserListItem RunInBackground(params UserListItem[] @params)
        {
            UserListItem user = @params[0];
            try
            {

                byte[] data = OperationWithBitmap.Retrieve(context, user.ID);

                if (data == null)
                {
                    user.image = OperationWithBitmap.GetFromUrl(user.url);
                    var bitmapData = OperationWithBitmap.ConvertToByteArray(user.image);
                    OperationWithBitmap.Cache(context, bitmapData, user.ID);
                }
                else
                {
                    user.image = OperationWithBitmap.ConvertFromByteArray(data);
                }
            }
            catch (Exception e) { Console.Out.WriteLine(e.Message); }

            return user;
        }

        protected override void OnPostExecute(UserListItem result)
        {
            adapter.NotifyDataSetChanged();
            base.OnPostExecute(result);
        }

    }

    public class CacheUserAvatar : AsyncTask<Profile, Java.Lang.Void, Bitmap>
    {
        private CircularImageView image;
        private Context context;
        private TextView initials;

        public CacheUserAvatar(CircularImageView image,TextView initials, Context context)
        {
            this.image = image;
            this.context = context;
            this.initials = initials;
        }

        protected override Android.Graphics.Bitmap RunInBackground(params Profile[] @params)
        {
            String imageUrl = @params[0].imageUri;
            String userId = @params[0].ID;
            Bitmap image = null;
            bool AvatarIsDefault = @params[0].AvatarIsDefault;

            if (imageUrl == null)
                return null;

            try
            {
                if (!AvatarIsDefault)
                {
                    byte[] data = OperationWithBitmap.Retrieve(context, userId);
                    initials.Visibility = ViewStates.Gone;

                    if (data == null)
                    {
                        image = OperationWithBitmap.GetFromUrl(imageUrl);
                        var bitmapData = OperationWithBitmap.ConvertToByteArray(image);
                        OperationWithBitmap.Cache(context, bitmapData, userId);
                    }
                    else
                    {
                        image = OperationWithBitmap.ConvertFromByteArray(data);
                    }
                }
                else
                {
                    initials.Text = AdditionalFunctions.DefineInitials(@params[0].name);
                    return null;
                }
            }
            catch (Exception e) { Console.Out.WriteLine(e.Message); }

            return image;
        }

        protected override void OnPostExecute(Bitmap result)
        {
            if (result != null)
            {
                image.SetImageBitmap(result);
            }
            else
            {
                //image.SetImageResource(Resource.Drawable.ic_noavatar);
                //VISIBLE LABLE WITH INITIALS (KK)
            }

            base.OnPostExecute(result);
        }
    }
}