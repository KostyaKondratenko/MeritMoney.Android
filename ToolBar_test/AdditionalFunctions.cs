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
using Android.Graphics;
using Java.IO;

namespace Merit_Money
{
    public static class AdditionalFunctions
    {
        public static DateTime FromUnixTime(String unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            try { epoch = epoch.AddSeconds(Convert.ToUInt64(unixTime)); }
            catch (OverflowException e) { System.Console.Out.WriteLine(e.Message); }
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

        public static int ConvertDpToPx(int padding_in_dp)
        {
            float scale = Application.Context.Resources.DisplayMetrics.Density;
            return (int)(padding_in_dp * scale + 0.5f);
        }

        public static Bitmap DrawTextToBitmap(Bitmap bitmap, String Text, int TextSize)
        {
            Bitmap.Config bitmapConfig =
                bitmap.GetConfig();
            // set default bitmap config if none
            if (bitmapConfig == null)
            {
                bitmapConfig = Bitmap.Config.Argb8888;
            }

            Canvas canvas = new Canvas(bitmap);
            // new antialised Paint
            Paint paint = new Paint(PaintFlags.AntiAlias);
            // text color - #3D3D3D
            paint.Color = Color.White;
            // text size in pixels
            paint.TextSize = TextSize;
            // text shadow
            paint.SetShadowLayer(1f, 0f, 1f, Color.Black);

            // draw text to the Canvas center
            Rect bounds = new Rect();
            paint.GetTextBounds(Text, 0, Text.Length, bounds);
            int x = (bitmap.Width - bounds.Width()) / 2;
            int y = (bitmap.Height + bounds.Height()) / 2;

            canvas.DrawText(Text, x, y, paint);

            return bitmap;
        }  
    }
}