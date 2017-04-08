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
                   jsonDoc["distribute"], jsonDoc["emailNotification"],
                   jsonDoc["webhookNotification"]);
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
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("Access-Token", CurrentAccessToken);
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
                        // Return the JSON document:
                        //return jsonDoc;
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
                        }

                        stream.Flush();
                        stream.Close();
                    }
                }

                JSONArray array = new JSONArray(jsonDoc.ToString());
                for (int i = 0; i < array.Length(); i++)
                {
                    JSONObject jsonobject = array.GetJSONObject(i);
                    String name = jsonobject.GetString("name");
                    String ID = jsonobject.GetString("ID");
                    String email = jsonobject.GetString("email");
                    String imUrl = jsonobject.GetString("imageUrl");
                    ListOfUsers.Add(new SingleUser(ID, name, email, imUrl));
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
    }
}