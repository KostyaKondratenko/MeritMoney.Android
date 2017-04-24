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
    public class HistoryObject
    {
        [PrimaryKey]
        public String ID { get; set; }
        public String toUserID { get; set; }
        public String fromUserID { get; set; }
        public int date { get; set; }
        public String message { get; set; }
        public String comment { get; set; }
        public HistoryObject()
        {
            ID = String.Empty;
            toUserID = String.Empty;
            fromUserID = String.Empty;
            date = 0;
            message = String.Empty;
            comment = String.Empty;
        }

        public HistoryObject(string id, string toUserID, string fromUserID, int date, string message, string comment)
        {
            ID = id;
            this.toUserID = toUserID;
            this.fromUserID = fromUserID;
            this.date = date;
            this.message = message;
            this.comment = comment;
        }
    }
}