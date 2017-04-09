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

namespace Merit_Money
{
    public class Profile
    {
        public String ID;
        public String name;
        public String email;
        public String imageUri;
        public int balance;
        public int rewards;
        public int distribute;
        public bool emailNotificaion;
        public bool webhookNotification;

        public Profile()
        {
            ID = String.Empty;
            name = "Igor Cubilya";
            email = String.Empty;
            imageUri = String.Empty;
            balance = 10;
            rewards = 20;
            distribute = 30;
            emailNotificaion = false;
            webhookNotification = false;
        }

        public Profile(string id, string name, string email, string url, int b, int r, int d, bool emailNot, bool webNot)
        {
            ID = id;
            this.name = name;
            this.email = email;
            imageUri = url;
            balance = b;
            rewards = r;
            distribute = d;
            emailNotificaion = emailNot;
            webhookNotification = webNot;
        }
    }

    public class SingleUser
    {
        public String ID { get; set; }
        public String name { get; set; }
        public String email { get; set; }
        public Android.Graphics.Bitmap image { get; set; }
        public String url { get; set; }

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