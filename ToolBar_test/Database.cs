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
                var connection = new SQLiteConnection(dbPath);
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

        public UsersDatabase()
        {
            String filename = Application.Context.GetString(Resource.String.UsersDBFilename);
            dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
        }

        public async Task CreateDatabase()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(dbPath);
                await connection.CreateTableAsync<UserListItem>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public async Task DeleteDatabase()
        {
            try
            {
                var connection = new SQLiteAsyncConnection(dbPath);
                await connection.DropTableAsync<UserListItem>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public async Task<bool> Insert(List<UserListItem> values)
        {
            try
            {
                var db = new SQLiteAsyncConnection(dbPath);
                await db.InsertAllAsync(values);
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> Merge(List<UserListItem> values)
        {
            try
            {
                var db = new SQLiteAsyncConnection(dbPath);
                foreach (UserListItem item in values)
                    if (await db.GetAsync<UserListItem>(item.ID) != null)
                        await db.UpdateAsync(item);
                    else
                        await db.InsertAsync(item);
                return true;
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
                var db = new SQLiteConnection(dbPath);
                String StrVal = user.AvatarIsDefault ? "1" : "0";

                db.Execute("UPDATE UserListItem SET AvatarIsDefault = ? WHERE ID = ?", StrVal, user.ID);
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public async Task<bool> Update(List<UserListItem> values)
        {
            try
            {
                await DeleteDatabase();
                await CreateDatabase();
                var db = new SQLiteAsyncConnection(dbPath);
                await db.InsertAllAsync(values);
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
                var ProfileDB = new ProfileDatabase();
                Profile p = ProfileDB.GetProfile();

                if (p.ID == ID)
                    return new UserListItem(p.ID, p.name, p.email, p.imageUri, null);

                var db = new SQLiteConnection(dbPath);
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

        public async Task<bool> IsExist()
        {
            List<UserListItem> result = new List<UserListItem>();
            try
            {
                var db = new SQLiteAsyncConnection(dbPath);
                var count = await db.ExecuteScalarAsync<int>("SELECT Count(*) FROM sqlite_master WHERE type = 'table' AND name = UserListItem");

                return count == 0 ? false : true;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<UserListItem>> GetUsers()
        {
            List<UserListItem> result = new List<UserListItem>();
            try
            {
                var db = new SQLiteAsyncConnection(dbPath);
                result = await db.Table<UserListItem>().ToListAsync();

                foreach (UserListItem user in result)
                    user.image = Android.Graphics.BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_noavatar);

                return result;
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return null;
            }
        }
    }

    public class HistoryDatabase
    {
        private String dbPath;
        private HistoryType type;

        public HistoryDatabase(HistoryType type)
        {
            String filename = String.Empty;
            this.type = type;

            switch (type)
            {
                case HistoryType.Company:
                    filename = Application.Context.GetString(Resource.String.PersonalHistoryDBFilename);
                    break;
                case HistoryType.Personal:
                    filename = Application.Context.GetString(Resource.String.CompanyHistoryDBFilename);
                    break;
            }

            dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), filename);
        }

        public void CreateDatabase()
        {
            try
            {
                var connection = new SQLiteConnection(dbPath);
                connection.CreateTable<HistoryListItem>();
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
                var connection = new SQLiteConnection(dbPath);
                connection.DropTable<HistoryListItem>();
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
        }

        public bool Insert(HistoryList values)
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

        public bool Update(List<HistoryListItem> values)
        {
            try
            {
                DeleteDatabase();
                CreateDatabase();
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

        public HistoryList GetUsers()
        {
            List<HistoryListItem> result = new List<HistoryListItem>();

            try
            {
                var db = new SQLiteConnection(dbPath);
                result = db.Table<HistoryListItem>().ToList();
                foreach (HistoryListItem user in result)
                {
                    user.image = Android.Graphics.BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_noavatar);
                }

                //SHARED PREFF
                bool hasMore = false;///

                return new HistoryList(result, hasMore, type);
            }
            catch (SQLiteException ex)
            {
                Console.Out.WriteLine(ex.Message);
                return null;
            }
        }
    }
}