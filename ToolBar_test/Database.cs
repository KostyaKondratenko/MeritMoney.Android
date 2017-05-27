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
using System.Threading.Tasks;

namespace Merit_Money
{
    public class ProfileDatabase
    {
        private String dbPath;

        public ProfileDatabase()
        {
            String filename = Application.Context.GetString(Resource.String.ProfileDBFilename);
            dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
        }

        public void CreateDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(dbPath))
                    connection.CreateTable<Profile>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public void DeleteDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(dbPath))
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
                using (var db = new SQLiteConnection(dbPath))
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
                using (var db = new SQLiteConnection(dbPath))
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
                using (var db = new SQLiteConnection(dbPath))
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

        public UsersDatabase()
        {
            String filename = Application.Context.GetString(Resource.String.UsersDBFilename);
            dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
        }

        public void CreateDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(dbPath))
                    connection.CreateTable<UserListItem>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public void DeleteDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(dbPath))
                    connection.DropTable<UserListItem>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public bool Insert(List<UserListItem> values)
        {
            try
            {
                using (var db = new SQLiteConnection(dbPath))
                    db.InsertAll(values);
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public bool Merge(List<UserListItem> values)
        {
            try
            {
                using (var db = new SQLiteConnection(dbPath))
                {
                    foreach (UserListItem item in values)
                        if (db.Get<UserListItem>(item.ID) != null)
                            db.Update(item);
                        else
                            db.Insert(item);

                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public void UpdateAvatarState(UserListItem user)
        {
            try
            {
                using (var db = new SQLiteConnection(dbPath))
                {
                    String StrVal = user.AvatarIsDefault ? "1" : "0";

                    db.Execute("UPDATE UserListItem SET AvatarIsDefault = ? WHERE ID = ?", StrVal, user.ID);
                }
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public bool Update(List<UserListItem> values)
        {
            try
            {
                using (var db = new SQLiteConnection(dbPath))
                    db.UpdateAll(values);
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public UserListItem GetUserByID(String ID)
        {
            try
            {
                //var ProfileDB = new ProfileDatabase();
                //Profile p = ProfileDB.GetProfile();

                //if (p.ID == ID)
                //    return new UserListItem(p.ID, p.name, p.email, p.imageUri, null);

                using (var db = new SQLiteConnection(dbPath))
                    return db.Get<UserListItem>(ID);
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return null;
            }
            catch (System.InvalidOperationException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return new UserListItem();
            }
        }

        public bool IsExist()
        {
            List<UserListItem> result = new List<UserListItem>();
            try
            {
                using (var db = new SQLiteConnection(dbPath))
                    //var count = await db.ExecuteAsync("SELECT Count(*) FROM sqlite_master WHERE type = 'table' AND name = UserListItem");
                    db.Query<int>("SELECT Count(*) FROM UserListItem WHERE ID = 1");
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<UserListItem> GetUsers()
        {
            List<UserListItem> result = new List<UserListItem>();
            try
            {
                using (var db = new SQLiteConnection(dbPath))
                    result = db.Table<UserListItem>().ToList();

                return result;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return null;
            }
        }
    }

    //public class HistoryDatabase
    //{
    //    private String dbPath;
    //    private HistoryType type;

    //    public HistoryDatabase(HistoryType type)
    //    {
    //        String filename = String.Empty;
    //        this.type = type;

    //        switch (type)
    //        {
    //            case HistoryType.Company:
    //                filename = Application.Context.GetString(Resource.String.PersonalHistoryDBFilename);
    //                break;
    //            case HistoryType.Personal:
    //                filename = Application.Context.GetString(Resource.String.CompanyHistoryDBFilename);
    //                break;
    //        }

    //        dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
    //    }

    //    public void CreateDatabase()
    //    {
    //        try
    //        {
    //            var connection = new SQLiteConnection(dbPath);
    //            connection.CreateTable<HistoryListItem>();
    //        }
    //        catch (SQLiteException ex)
    //        {
    //            Console.Out.WriteLine(ex.Message);
    //        }
    //    }

    //    public void DeleteDatabase()
    //    {
    //        try
    //        {
    //            var connection = new SQLiteConnection(dbPath);
    //            connection.DropTable<HistoryListItem>();
    //        }
    //        catch (SQLiteException ex)
    //        {
    //            Console.Out.WriteLine(ex.Message);
    //        }
    //    }

    //    public bool Insert(HistoryList values)
    //    {
    //        try
    //        {
    //            var db = new SQLiteConnection(dbPath);
    //            db.InsertAll(values);
    //            return true;
    //        }
    //        catch (SQLiteException ex)
    //        {
    //            Console.Out.WriteLine(ex.Message);
    //            return false;
    //        }
    //    }

    //    public bool Update(List<HistoryListItem> values)
    //    {
    //        try
    //        {
    //            DeleteDatabase();
    //            CreateDatabase();
    //            var db = new SQLiteConnection(dbPath);
    //            db.InsertAll(values);
    //            return true;
    //        }
    //        catch (SQLiteException ex)
    //        {
    //            Console.Out.WriteLine(ex.Message);
    //            return false;
    //        }
    //    }

    //    public HistoryList GetUsers()
    //    {
    //        List<HistoryListItem> result = new List<HistoryListItem>();

    //        try
    //        {
    //            var db = new SQLiteConnection(dbPath);
    //            result = db.Table<HistoryListItem>().ToList();

    //            bool hasMore = false;///

    //            return new HistoryList(result, hasMore, type);
    //        }
    //        catch (SQLiteException ex)
    //        {
    //            Console.Out.WriteLine(ex.Message);
    //            return null;
    //        }
    //    }
    //}
}