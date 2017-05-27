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

        public static Bitmap DrawTextToBitmap(String Text, int TextSize)
        {
            Android.Content.Res.Resources resources = Application.Context.Resources;
            float scale = resources.DisplayMetrics.Density;
            Bitmap bitmap = BitmapFactory.DecodeResource(resources, Resource.Drawable.ic_noavatar);

            Bitmap.Config bitmapConfig =
                bitmap.GetConfig();
            // set default bitmap config if none
            if (bitmapConfig == null)
            {
                bitmapConfig = Bitmap.Config.Argb8888;
            }
            // resource bitmaps are imutable, 
            // so we need to convert it to mutable one
            //bitmap = bitmap.Copy(bitmapConfig, true);

            Canvas canvas = new Canvas(bitmap);
            // new antialised Paint
            Paint paint = new Paint(PaintFlags.AntiAlias);
            // text color - #3D3D3D
            paint.Color = Color.White;
            // text size in pixels
            paint.TextSize = (int)(TextSize * scale);
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

        //private static Bitmap convertToMutable(Bitmap imgIn)
        //{
        //    try
        //    {
        //        //this is the file going to use temporally to save the bytes. 
        //        // This file will not be a image, it will store the raw image data.
        //        File file = new File(Android.OS.Environment.GetExternalStorage + File.Separator + "temp.tmp");

        //        //Open an RandomAccessFile
        //        //Make sure you have added uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE"
        //        //into AndroidManifest.xml file
        //        RandomAccessFile randomAccessFile = new RandomAccessFile(file, "rw");

        //        // get the width and height of the source bitmap.
        //        int width = imgIn.getWidth();
        //        int height = imgIn.getHeight();
        //        Config type = imgIn.getConfig();

        //        //Copy the byte to the file
        //        //Assume source bitmap loaded using options.inPreferredConfig = Config.ARGB_8888;
        //        FileChannel channel = randomAccessFile.getChannel();
        //        MappedByteBuffer map = channel.map(MapMode.READ_WRITE, 0, imgIn.getRowBytes() * height);
        //        imgIn.copyPixelsToBuffer(map);
        //        //recycle the source bitmap, this will be no longer used.
        //        imgIn.recycle();
        //        System.gc();// try to force the bytes from the imgIn to be released

        //        //Create a new bitmap to load the bitmap again. Probably the memory will be available. 
        //        imgIn = Bitmap.createBitmap(width, height, type);
        //        map.position(0);
        //        //load it back from temporary 
        //        imgIn.copyPixelsFromBuffer(map);
        //        //close the temporary file and channel , then delete that also
        //        channel.close();
        //        randomAccessFile.close();

        //        // delete the temp file
        //        file.delete();

        //    }
        //    catch (FileNotFoundException e)
        //    {
        //        e.printStackTrace();
        //    }
        //    catch (IOException e)
        //    {
        //        e.printStackTrace();
        //    }

        //    return imgIn;
        //}
    }
}