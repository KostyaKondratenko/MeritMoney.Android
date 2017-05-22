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
using System.Json;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Org.Json;
using Merit_Money;
using System.Threading;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Client;
using Org.Apache.Http.Client.Methods;
using Android.Graphics;

namespace Merit_Money
{
    public static class MeritMoneyBrain
    {
        public static String CurrentAccessToken;
        private const String MeritMoneyApiUrl = "https://apitest-dot-practice-meritmoney-157913.appspot.com/externalapi/";
        private static Profile profile;

        //Find out how to save AccessToken

        public static async Task AccessToken(String email)
        {
            try
            {
                // Create an HTTP web request using the URL:
                string url = MeritMoneyApiUrl + "loginEmail";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string jsonData = @"{""email"":" + "\"" + email + "\"" + "}";
                    streamWriter.Write(jsonData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                JsonValue jsonDoc;

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                        if (string.IsNullOrWhiteSpace(jsonDoc.ToString()))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            Console.Out.WriteLine("Response Body: \r\n {0}", jsonDoc.ToString());
                            CurrentAccessToken = jsonDoc["accessToken"];
                        }

                        stream.Flush();
                        stream.Close();
                        // Return the JSON document:
                        //return jsonDoc;
                    }
                }
            }
            catch (WebException exception)
            {
                string responseText;
                Console.WriteLine(exception.Message);
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task SingInWithGoogle(String token)
        {
            try
            {
                // Create an HTTP web request using the URL:
                string url = MeritMoneyApiUrl + "login";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //string jsonData = @"{""token"":" + "\""  token + "\"" + "}";
                    string jsonData = "{\"token\":\"" + token + "\"}";

                    streamWriter.Write(jsonData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                JsonValue jsonDoc;

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                        if (string.IsNullOrWhiteSpace(jsonDoc.ToString()))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            Console.Out.WriteLine("Response Body: \r\n {0}", jsonDoc.ToString());
                            CurrentAccessToken = jsonDoc["accessToken"];
                        }

                        stream.Flush();
                        stream.Close();
                        // Return the JSON document:
                        //return jsonDoc;
                    }
                }
            }
            catch (WebException exception)
            {
                string responseText;
                Console.WriteLine(exception.Message);
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                    await ProcessingError(responseText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task<Profile> GetProfile()
        {
            try
            {
                // Create an HTTP web request using the URL:
                //url += "profile?Access-Token=" + Token;
                string url = MeritMoneyApiUrl + "profile";
                //url += "profile";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Headers.Add("Access-Token", CurrentAccessToken);
                request.Method = "GET";

                JsonValue jsonDoc;

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                        if (string.IsNullOrWhiteSpace(jsonDoc.ToString()))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            Console.Out.WriteLine("Response Body: \r\n {0}", jsonDoc.ToString());
                        }

                        stream.Flush();
                        stream.Close();
                        // Return the JSON document:
                        //return jsonDoc;
                    }
                }
                profile = new Profile(jsonDoc["ID"], jsonDoc["name"],
                    jsonDoc["email"], jsonDoc["imageUrl"],
                   jsonDoc["balance"], jsonDoc["rewards"],
                   jsonDoc["distribute"], jsonDoc["emailNotification"]);
            }
            catch (WebException exception)
            {
                string responseText;
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                    await ProcessingError(responseText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return profile;
        }

        public static async Task LogOut()
        {
            try
            {
                // Create an HTTP web request using the URL:
                string url = MeritMoneyApiUrl + "logout";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Access-Token", CurrentAccessToken);
                request.ContentLength = 0;
                request.Method = "POST";

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:

                        if (response.ContentLength == 0)
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            Console.Out.WriteLine("LOGGED OUT");
                        }

                        stream.Flush();
                        stream.Close();
                    }
                }
            }
            catch (WebException exception)
            {
                string responseText;
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                    await ProcessingError(responseText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task<List<UserListItem>> GetListOfUsers(String modifyAfter)
        {
            List<UserListItem> ListOfUsers = new List<UserListItem>();
            ProfileDatabase db = new ProfileDatabase();
            Profile p = db.GetProfile();
            String currentID = p.ID;
            String AdministratorID = "0000";

            try
            {
                // Create an HTTP web request using the URL:
                string url = MeritMoneyApiUrl + "users";

                if (modifyAfter != String.Empty)
                    url += "?modifyAfter=" + modifyAfter;

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Headers.Add("Access-Token", CurrentAccessToken);
                request.Method = "GET";

                JsonValue jsonDoc;
                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        jsonDoc = await Task.Run(() => JsonValue.Load(stream));

                        if (string.IsNullOrWhiteSpace(jsonDoc.ToString()))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            Console.Out.WriteLine("Response Body: \r\n {0}", jsonDoc.ToString());
                            JSONArray array = new JSONArray(jsonDoc.ToString());

                            for (int i = 0; i < array.Length(); i++)
                            {
                                JSONObject jsonobject = array.GetJSONObject(i);
                                String name = jsonobject.GetString("name");
                                String ID = jsonobject.GetString("ID");
                                String email = jsonobject.GetString("email");
                                String imUrl = jsonobject.GetString("imageUrl");

                                if (ID != currentID && ID != AdministratorID)
                                    ListOfUsers.Add(new UserListItem(ID, name, email, imUrl, null));
                            }
                             
                            Int64 currentUtcTime = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            ISharedPreferences info = Application.Context.GetSharedPreferences(Application.Context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                            ISharedPreferencesEditor edit = info.Edit();
                            edit.PutString(Application.Context.GetString(Resource.String.ModifyDate), currentUtcTime.ToString());
                            edit.Apply();
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                string responseText;
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                    await ProcessingError(responseText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ListOfUsers;
        }

        public static async Task<Profile> DistributePoints(String points, String userId, String message)
        {
            try
            {
                // Create an HTTP web request using the URL:
                string url = MeritMoneyApiUrl + "distributePoints";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add("Access-Token", CurrentAccessToken);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string jsonData = @"{""userID"":" + "\"" + userId + "\"" + "," +
                                      @"""points"":" + points + "," +
                                      @"""comment"":" + "\"" + message + "\"" + "}";
                    streamWriter.Write(jsonData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                JsonValue jsonDoc;

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                        if (string.IsNullOrWhiteSpace(jsonDoc.ToString()))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            Console.Out.WriteLine("Response Body: \r\n {0}", jsonDoc.ToString());
                        }

                        stream.Flush();
                        stream.Close();
                        // Return the JSON document:
                        //return jsonDoc;
                    }
                }
                profile = new Profile(jsonDoc["ID"], jsonDoc["name"],
                  jsonDoc["email"], jsonDoc["imageUrl"],
                 jsonDoc["balance"], jsonDoc["rewards"],
                 jsonDoc["distribute"], jsonDoc["emailNotification"]);
            }
            catch (WebException exception)
            {
                string responseText;
                Console.WriteLine(exception.Message);
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                    await ProcessingError(responseText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return profile;
        }

        public static async Task<HistoryList> GetHistory(int offset, int batchSize, HistoryType type)
        {
            HistoryList HistoryList = new HistoryList(type);

            string histType = (type == HistoryType.Personal) ? "personal" : "company";

            try
            {
                // Create an HTTP web request using the URL:
                string url = MeritMoneyApiUrl + histType + "History" + "?offset=" + offset.ToString() +
                    "&batchSize=" + batchSize.ToString();
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Headers.Add("Access-Token", CurrentAccessToken);
                request.Method = "GET";

                JsonValue jsonDoc;

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                        if (string.IsNullOrWhiteSpace(jsonDoc.ToString()))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            Console.Out.WriteLine("Response Body: \r\n {0}", jsonDoc.ToString());

                            bool hasMore = jsonDoc["hasMore"];
                            jsonDoc = jsonDoc["history"];

                            HistoryList.hasMore = hasMore;

                            JSONArray array = new JSONArray(jsonDoc.ToString());
                            for (int i = 0; i < array.Length(); i++)
                            {
                                JSONObject jsonobject = array.GetJSONObject(i);
                                String ID = jsonobject.GetString("ID");
                                String toUserID = jsonobject.GetString("toUserID");
                                String fromUserID = jsonobject.GetString("fromUserID");
                                int date = jsonobject.GetInt("date");
                                String message = jsonobject.GetString("message");
                                String comment = jsonobject.GetString("comment");

                                HistoryList.Add(new HistoryListItem(ID, toUserID, fromUserID, date, message, comment, null));
                            }
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                string responseText;
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                    await ProcessingError(responseText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return HistoryList;
        }

        public static async Task<Profile> updateProfile(String name, bool emailNotificationWasChanged, bool value)
        {
            try
            {
                // Create an HTTP web request using the URL:
                string url = MeritMoneyApiUrl + "updateProfile";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add("Access-Token", CurrentAccessToken);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string jsonData = "{";
                    if (name != String.Empty)
                    {
                        jsonData += "\"name:\":\"" + name + "\"";
                    }
                    if (name != String.Empty && emailNotificationWasChanged) { jsonData += ","; }
                    if (emailNotificationWasChanged)
                    {
                        int val = value ? 1 : 0;
                        jsonData += "\"emailNotification\":" + val.ToString();
                    }
                    jsonData += "}";
                    streamWriter.Write(jsonData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                JsonValue jsonDoc;

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON document object:
                        jsonDoc = await Task.Run(() => JsonObject.Load(stream));

                        if (string.IsNullOrWhiteSpace(jsonDoc.ToString()))
                        {
                            Console.Out.WriteLine("Response contained empty body...");
                        }
                        else
                        {
                            Console.Out.WriteLine("Response Body: \r\n {0}", jsonDoc.ToString());
                        }

                        stream.Flush();
                        stream.Close();
                        // Return the JSON document:
                        //return jsonDoc;
                    }
                }
                profile = new Profile(jsonDoc["ID"], jsonDoc["name"],
                    jsonDoc["email"], jsonDoc["imageUrl"],
                   jsonDoc["balance"], jsonDoc["rewards"],
                   jsonDoc["distribute"], jsonDoc["emailNotification"]);
            }
            catch (WebException exception)
            {
                string responseText;
                Console.WriteLine(exception.Message);
                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                    Console.WriteLine(responseText);
                    await ProcessingError(responseText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return profile;
        }

        private static async Task ProcessingError(String error)
        {
            JsonValue jsonDoc = JsonValue.Parse(error);
            String curError = jsonDoc["error"];
            switch (curError)
            {
                case "BadInputParameters":
                    Toast.MakeText(Application.Context,
                        "Some required parameters are missing or have incorrect data type.",
                        ToastLength.Short).Show();
                    break;
                case "InsufficientAmount":
                    break;
                case "BadTokenID":
                    break;
                case "BadAccessToken":
                    Toast.MakeText(Application.Context,
                        "Access token invalid or malformed.",
                        ToastLength.Short).Show();
                    await Task.Delay(3000);
                    await LogOut();
                    break;
                case "AccessDenied":
                    Toast.MakeText(Application.Context,
                        "You are not allowed to use the app. Please try login under another account.",
                        ToastLength.Long).Show();
                    break;
                case "DatabaseError":
                    Toast.MakeText(Application.Context,
                        "Merit Money database error.",
                        ToastLength.Long).Show();
                    ISharedPreferences info = Application.Context.GetSharedPreferences(Application.Context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
                    ISharedPreferencesEditor edit = info.Edit();
                    edit.Remove(Application.Context.GetString(Resource.String.ModifyDate));
                    edit.Apply();
                    break;
                case "InternalError":
                    Toast.MakeText(Application.Context,
                        "Merit Money Internal server error occured.",
                        ToastLength.Long).Show();
                    break;
            }
        }
    }
}