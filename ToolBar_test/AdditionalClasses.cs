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
using SQLite;
using System.Collections;

namespace Merit_Money
{
    public class Profile
    {
        [PrimaryKey]
        public String ID { get; set; }
        public String name { get; set; }
        public String email { get; set; }
        public bool AvatarIsDefault { get; set; }
        public String imageUri { get; set; }
        public int balance { get; set; }
        public int rewards { get; set; }
        public int distribute { get; set; }
        public bool emailNotificaion { get; set; }

        public Profile()
        {
            ID = String.Empty;
            name = "User name";
            email = String.Empty;
            imageUri = String.Empty;
            balance = 0;
            rewards = 0;
            distribute = 0;
            emailNotificaion = false;
            AvatarIsDefault = false;
        }

        public Profile(string id, string name, string email, string url, int b, int r, int d, bool emailNot)
        {
            ID = id;
            this.name = name;
            this.email = email;
            imageUri = url;
            balance = b;
            rewards = r;
            distribute = d;
            emailNotificaion = emailNot;
            AvatarIsDefault = false;
        }

        public static implicit operator Profile(List<Profile> v)
        {
            return v[0];
        }
    }


    public class HistoryListItem
    {
        [PrimaryKey]
        public String ID { get; set; }
        public String toUserID { get; set; }
        public String fromUserID { get; set; }
        public int date { get; set; }
        [Ignore]
        public Android.Graphics.Bitmap image { get; set; }
        public String message { get; set; }
        public String comment { get; set; }

        public HistoryListItem()
        {
            ID = String.Empty;
            toUserID = String.Empty;
            fromUserID = String.Empty;
            date = 0;
            message = String.Empty;
            comment = String.Empty;
            image = null;
        }

        public HistoryListItem(string id, string toUserID, string fromUserID, int date, string message, string comment, Android.Graphics.Bitmap image)
        {
            ID = id;
            this.toUserID = toUserID;
            this.fromUserID = fromUserID;
            this.date = date;
            this.message = message;
            this.comment = comment;
            this.image = image;
        }
    }

    public class HistoryList : IEnumerable<HistoryListItem>
    {
        public HistoryType type { get; set; }
        public List<HistoryListItem> ListOfHistory;
        public Boolean hasMore { get; set; }

        public HistoryList(HistoryType type)
        {
            this.type = type;
            ListOfHistory = new List<HistoryListItem>();
            hasMore = false;
        }

        public HistoryList(List<HistoryListItem> list, Boolean hasMore, HistoryType type)
        {
            this.ListOfHistory = list;
            this.hasMore = hasMore;
            this.type = type;
        }

        public void Clear()
        {
            ListOfHistory.Clear();
            hasMore = false;
        }

        public void KeepItemsInMemory(int count)
        {
            List<HistoryListItem> tmp = ListOfHistory.GetRange(0, count);
            ListOfHistory.Clear();
            ListOfHistory = tmp;
        }

        public void AddList(HistoryList list)
        {
            ListOfHistory.AddRange(list);
            hasMore = list.hasMore;
        }

        public void Add(HistoryListItem value)
        {
            ListOfHistory.Add(value);
        }

        public int Count()
        {
            return ListOfHistory.Count;
        }

        public IEnumerator<HistoryListItem> GetEnumerator()
        {
            return ListOfHistory.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public HistoryListItem this[int key]
        {
            get
            {
                try
                {
                    return ListOfHistory[key];
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }

            }
        }
    }

    public class UserListItem
    { 
        [PrimaryKey]
        public String ID { get; set; }
        public String name { get; set; }
        public String email { get; set; }
        [Ignore]
        public Android.Graphics.Bitmap image { get; set; }
        public bool AvatarIsDefault { get; set; }
        public String url { get; set; }

        public UserListItem()
        {
            ID = String.Empty;
            name = "Unknown user";
            email= String.Empty;
            image = null;
            url= String.Empty;
            AvatarIsDefault = true;
        }

        public UserListItem(String ID, String name, String email,String url, Android.Graphics.Bitmap image)
        {
            this.ID = ID;
            this.name = name;
            this.email = email;
            this.image = image;
            this.url = url;
            AvatarIsDefault = true;
        }
    }
}