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

namespace Merit_Money
{
    public class Profile
    {
        [PrimaryKey]
        public String ID { get; set; }
        public String name { get; set; }
        public String email { get; set; }
        //[Ignore]
        //public Android.Graphics.Bitmap image { get; set; }
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
            //image = null;
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
        }

        public static implicit operator Profile(List<Profile> v)
        {
            return v[0];
        }
    }

    public class SingleUser
    { 
        [PrimaryKey]
        public String ID { get; set; }
        public String name { get; set; }
        public String email { get; set; }
        [Ignore]
        public Android.Graphics.Bitmap image { get; set; }
        public String url { get; set; }

        public SingleUser()
        {
            ID = String.Empty;
            name = String.Empty;
            email= String.Empty;
            image = null;
            url= String.Empty;
        }

        public SingleUser(String ID, String name, String email,String url, Android.Graphics.Bitmap image)
        {
            this.ID = ID;
            this.name = name;
            this.email = email;
            this.image = image;
            this.url = url;
        }
    }
}