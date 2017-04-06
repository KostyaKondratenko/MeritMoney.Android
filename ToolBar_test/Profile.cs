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

namespace ToolBar_test
{
    public class ProfileClass
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

        public ProfileClass()
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

        public ProfileClass(string id, string name, string email, string url, int b, int r, int d, bool emailNot, bool webNot)
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
}