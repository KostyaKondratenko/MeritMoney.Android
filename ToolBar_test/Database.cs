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
using System.IO;

namespace Merit_Money
{
    public class ProfileDatabase
    {
        private String dbPath;

        public ProfileDatabase(String filename)
        {
            dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
        }

        public void createDatabase()
        {
            try
            {
                var connection = new SQLiteConnection(dbPath);
                connection.CreateTable<Profile>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public void deleteDatabase()
        {
            try
            {
                var connection = new SQLiteConnection(dbPath);
                connection.DropTable<Profile>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public void Insert(Profile value)
        {
            try
            {
                var db = new SQLiteConnection(dbPath);
                db.Insert(value);
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public void Update(Profile value)
        {
            try
            {
                var db = new SQLiteConnection(dbPath);
                db.Update(value);
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public Profile GetProfile()
        {
            Profile result = new Profile();
            try
            {
                var db = new SQLiteConnection(dbPath);
                result = db.Table<Profile>().ToList();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
            return result;
        }
    }

    public class UsersDatabase
    {
        private String dbPath;

        public UsersDatabase(String filename)
        {
            dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
        }

        public void createDatabase()
        {
            try
            {
                var connection = new SQLiteConnection(dbPath);
                connection.CreateTable<SingleUser>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public void deleteDatabase()
        {
            try
            {
                var connection = new SQLiteConnection(dbPath);
                connection.DropTable<SingleUser>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public bool Insert(List<SingleUser> values)
        {
            try
            {
                var db = new SQLiteConnection(dbPath);
                db.InsertAll(values);
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public bool Update(List<SingleUser> values)
        {
            try
            {
                deleteDatabase();
                createDatabase();
                var db = new SQLiteConnection(dbPath);
                db.InsertAll(values);
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public List<SingleUser> GetUsers()
        {
            List<SingleUser> result = new List<SingleUser>();
            try
            {
                var db = new SQLiteConnection(dbPath);
                result = db.Table<SingleUser>().ToList();
                foreach (SingleUser user in result)
                {
                    user.image = Android.Graphics.BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_noavatar);
                }
                return result;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return null;
            }
        }
    }
}