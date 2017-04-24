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
                    string jsonData = "{\"token\":\"" + token +"\"}";

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
                }
            }
            catch(Exception e)
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
                request.ContentType = "application/json";
                request.Headers.Add("Access-Token", CurrentAccessToken);
                //request.ContentLength = UTF8Encoding.Default.GetBytes(CurrentAccessToken).Length;
                request.Method = "POST";

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
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task<List<SingleUser>> GetListOfUsers()
        {
            List<SingleUser> ListOfUsers = new List<SingleUser>();
            ISharedPreferences info = Application.Context.GetSharedPreferences(Application.Context.GetString(Resource.String.ApplicationInfo), FileCreationMode.Private);
            String currentID = info.GetString(Application.Context.GetString(Resource.String.ID), String.Empty);
            try
            {
                // Create an HTTP web request using the URL:
                string url = MeritMoneyApiUrl + "users";
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
                            Android.Graphics.Bitmap img = Android.Graphics.BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.ic_noavatar);
                            for (int i = 0; i < array.Length(); i++)
                            {
                                JSONObject jsonobject = array.GetJSONObject(i);
                                String name = jsonobject.GetString("name");
                                String ID = jsonobject.GetString("ID");
                                String email = jsonobject.GetString("email");
                                String imUrl = jsonobject.GetString("imageUrl");

                                if (ID != currentID && ID != String.Empty)
                                {
                                    ListOfUsers.Add(new SingleUser(ID, name, email, imUrl, img));
                                }
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ListOfUsers;
        }

        public static async Task DistributePoints(String points, String userId, String message)
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
                                      @"""points"":" +  points + "," +
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
        }

        public static async Task<List<HistoryObject>> GetHistory(int offset, int batchSize, HistoryType type)
        {
            List<HistoryObject> listOfHistoryObjects = new List<HistoryObject>();
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

                                listOfHistoryObjects.Add(new HistoryObject(ID, toUserID, fromUserID, date, message, comment));
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return listOfHistoryObjects;
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
                    if(name!=String.Empty && emailNotificationWasChanged) { jsonData += ","; }
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
                }
            }
            return profile;
        }

        public static Bitmap ReadFromInternalStorage(String userId)
        {
            var sdCardPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(sdCardPath, userId);

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(filePath, options);

            return bitmap;
        }

        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new System.Net.WebClient())
            {
                if (url != String.Empty)
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