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

namespace Merit_Money
{
    public static class OperationWithBitmap
    {
        private const long MAX_CACHE_SIZE = 5242880L;

        public static byte[] ConvertToByteArray(Bitmap bitmap)
        {
            byte[] bitmapData;
            using (var stream = new System.IO.MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                bitmapData = stream.ToArray();
            }
            return bitmapData;
        }

        public static Bitmap ConvertFromByteArray(byte[] data)
        {
            var imageBitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            return imageBitmap;
        }

        public static void Cache(Context context, byte[] data, String name)
        {

            Java.IO.File cacheDir = context.CacheDir;
            long size = GetDirSize(cacheDir);
            long newSize = data.Length + size;

            if (newSize > MAX_CACHE_SIZE)
            {
                CleanDir(cacheDir, newSize - MAX_CACHE_SIZE);
            }

            Java.IO.File file = new Java.IO.File(cacheDir, name);
            Java.IO.FileOutputStream os = new Java.IO.FileOutputStream(file);
            try
            {
                os.Write(data);
            }
            finally
            {
                os.Flush();
                os.Close();
            }
        }

        public static byte[] Retrieve(Context context, String name)
        {
            Java.IO.File cacheDir = context.CacheDir;
            Java.IO.File file = new Java.IO.File(cacheDir, name);

            if (!file.Exists())
            {
                return null;
            }

            byte[] data = new byte[(int)file.Length()];
            Java.IO.FileInputStream stream = new Java.IO.FileInputStream(file);
            try
            {
                stream.Read(data);
            }
            finally
            {
                stream.Close();

            }
            return data;
        }

        private static void CleanDir(Java.IO.File dir, long bytes)
        {

            long bytesDeleted = 0;
            Java.IO.File[] files = dir.ListFiles();

            foreach (Java.IO.File file in files)
            {
                bytesDeleted += file.Length();
                file.Delete();

                if (bytesDeleted >= bytes)
                {
                    break;
                }
            }
        }

        private static long GetDirSize(Java.IO.File dir)
        {

            long size = 0;
            Java.IO.File[] files = dir.ListFiles();

            foreach (Java.IO.File file in files)
            {
                if (file.IsFile)
                {
                    size += file.Length();
                }
            }

            return size;
        }

        public static Bitmap ReadFromInternalStorage(String userId)
        {
            var sdCardPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            //var sdCardPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var filePath = System.IO.Path.Combine(sdCardPath, userId);

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(filePath, options);

            return bitmap;
        }

        public static bool isDefault(Bitmap curBitmap)
        {
            Bitmap Default = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_noavatar);

            if (Default.Width != curBitmap.Width
                || Default.Height != curBitmap.Height)
                return false;

            byte[] cur = ConvertToByteArray(curBitmap);
            byte[] def = ConvertToByteArray(Default);

            return cur.SequenceEqual(def);
        }

        public static bool isDefault(byte[] curImage)
        {
            Bitmap Default = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_noavatar);

            byte[] def = ConvertToByteArray(Default);

            return curImage.SequenceEqual(def);
        }

        public static bool isDefault(String url)
        {
            Bitmap image = GetFromUrl(url);
            return isDefault(image);
        }

        public static Bitmap GetFromUrl(String url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new System.Net.WebClient())
            {
                if (url != String.Empty && url != null)
                {
                    try
                    {
                        var imageBytes = webClient.DownloadData(url);
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            imageBitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        }
                    }
                    catch (System.Net.WebException)
                    {
                        return null;
                    }
                    catch (Java.Lang.OutOfMemoryError)
                    {
                        return null;
                    }
                }
            }
            return imageBitmap;
        }
    }
}