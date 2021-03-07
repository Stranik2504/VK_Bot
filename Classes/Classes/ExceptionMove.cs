using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using VkNet;
using Newtonsoft.Json;
using VkNet.Model.RequestParams;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;

namespace Classes
{
    public class ExceptionMove
    {
        private static Mutex mutex = new Mutex();
        private static string namefile = "Exception.log";
        private static string clientId = "";
        private static string clientSecret = "";

        public static void Exception(string logMessage, VkApi bot)
        {
            if (ConfigMeneger.Configth == null || ConfigMeneger.Configth.IsActivateExceptionMove == true)
            {
                try
                {
                    mutex.WaitOne();

                    File.AppendAllText(namefile, logMessage + "\n");

                    mutex.ReleaseMutex();
                }
                catch (Exception ex) { try { Logger.Log($"Error to save({ex.Message}({DateTime.Now}))"); } catch { } try { mutex.ReleaseMutex(); } catch { } }

                try
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = 193292716, Message = $"Error({DateTime.Now}): {logMessage}", RandomId = new Random().Next() });
                }
                catch (Exception ex) { try { Logger.Log($"Error sending an error({ex.Message}({DateTime.Now}))"); } catch { } try { mutex.ReleaseMutex(); } catch { } }

                try
                {
                    if (File.Exists(namefile) == true)
                    {
                        using (StreamReader reader = new StreamReader("disk.json"))
                        {
                            var paramseters = JsonConvert.DeserializeObject<dynamic>(reader.ReadLine());

                            try { clientId = paramseters["installed"]["client_id"]; } catch { clientId = "563423904039-4nvb86di72588du4nq6of9us9d56vrjc.apps.googleusercontent.com"; }
                            try { clientSecret = paramseters["installed"]["client_secret"]; } catch { clientSecret = "pDu1-2krRPZ4yHWEb2u0WdFT"; }

                            Authorize(namefile);
                        }
                    }
                }
                catch (Exception ex) { try { Logger.Log($"Error sending on disk an error({ex.Message}({DateTime.Now}))"); } catch { } try { mutex.ReleaseMutex(); } catch { } }
            }
        }

        private static void Authorize(string namefile)
        {
            string[] scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile,};
                                                                         
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            }, scopes,
            Environment.UserName, CancellationToken.None, new FileDataStore("MyAppsToken")).Result;   

            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MyAppName",

            });
            service.HttpClient.Timeout = TimeSpan.FromMinutes(100);

            var respocne = uploadFile(service, namefile, "");
        }

        private static Google.Apis.Drive.v3.Data.File uploadFile(DriveService service, string file, string peren)
        {
            if (File.Exists(file))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = Path.GetFileName(file);
                body.Description = "Error reload";
                body.MimeType = GetMimeType(file);
                byte[] byteArray = File.ReadAllBytes(file);
                MemoryStream stream = new MemoryStream(byteArray);
                try
                {
                    FilesResource.UpdateMediaUpload request = service.Files.Update(body, "163U9eE-r7EhCcIAK6nUhgsRK_X_upwKz", stream, GetMimeType(file));
                    request.SupportsTeamDrives = true;
                    request.Upload();
                    return request.ResponseBody;
                }
                catch { return null; }
            }

            return null;
        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}
