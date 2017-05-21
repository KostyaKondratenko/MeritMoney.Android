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
using System.Text.RegularExpressions;

namespace Merit_Money
{
    public static class AdditionalFunctions
    {
        public static DateTime FromUnixTime(String unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            try { epoch = epoch.AddSeconds(Convert.ToUInt64(unixTime)); }
            catch (OverflowException e) { Console.Out.WriteLine(e.Message); }
            return epoch;
        }

        public static String DefineInitials(String fullName)
        {
            String initials = String.Empty;
            String[] name = Regex.Split(fullName, @"\W+");
            for (int i = 0; i < name.Length && i < 3; i++)
                initials += name[i][0];
            return initials;
        }
    }
}