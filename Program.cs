using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace gPhotoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string credPath = @"C:Users\han\Code\gPhotoConsole\result.txt\";
            clsResponseRootObject responseObject = new clsResponseRootObject();
            UserCredential credential;
            string[] scopes = {
                "https://www.googleapis.com/auth/photoslibrary.sharing",
                "https://www.googleapis.com/auth/photoslibrary.readonly"
            };
            string UserName = "hanyusok";
            string ClientID = "94253113748-2qrrpnseg8n724doa6kfislc2airjg7d.apps.googleusercontent.com";
            string ClientSecret = "eR2WjLaIH7MyFtDUDudqKsdc";

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    UserName,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://photoslibrary.googleapis.com/v1/mediaItems");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("client_id", ClientID);
                httpWebRequest.Headers.Add("client_secret", ClientSecret);
                httpWebRequest.Headers.Add("Authorization:" + credential.Token.TokenType + " " + credential.Token.AccessToken);
                httpWebRequest.Method = "GET";

                HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

                responseObject = JsonConvert.DeserializeObject<clsResponseRootObject>(reader.ReadToEnd());

                if (responseObject != null)
                {
                    if (responseObject.mediaItems != null && responseObject.mediaItems.Count > 0)
                    {
                        Console.WriteLine("------------------------Retrieving media files--------------------------------");
                        foreach (var item in responseObject.mediaItems)
                        { 
                            Console.WriteLine(string.Format("ID:{0}, Filename:{1}, MimeType:{2}", item.id, item.filename, item.mimeType));
                        }
                    }
                }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: " + ex.Message);
            }

            Console.ReadLine();
        }
    }
}
