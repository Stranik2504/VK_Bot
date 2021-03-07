using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Command_List;
using Classes;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace VK_Bot.Classes
{
    public static class SendMessageTime
    {
        private static bool isActivate = false;
        private static Task sendMess;
        public static VkApi bot;

        public static void Start()
        {
            try
            {
                isActivate = true;
                sendMess = Task.Factory.StartNew(Send);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[Send message activate]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][Send message activate]");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(send message start)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(send message start)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(send message start)]: {ex.Message}", null);
            }
        }

        public static void Stop()
        {
            try
            {
                isActivate = false;

                sendMess.Wait();
                sendMess.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[Send message stoped]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][Send message stoped]");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(send message stoped)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(send message stoped)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(send message stoped)]: {ex.Message}", null);
            }
        }

        public static void Update(VkApi newbot)
        {
            bot = newbot;
        }

        public static bool Answer(string answer, long userId)
        {
            try
            {
                if (ConfigMeneger.Configth.ActivateMessageSend == true)
                {
                    if (ConfigMeneger.Configth.GetAnswer.Split(')')[0].Length > 0)
                    {
                        if (ConfigMeneger.Configth.GetAnswer.Remove(0, (ConfigMeneger.Configth.GetAnswer.Split(')')[0] + ")" + " ").Length) == answer)
                        {
                            foreach (var id in GetAllUserId(bot))
                            {
                                if (userId == (long)Convert.ToInt64(id.Split(' ')[0]))
                                {
                                    int numPoint = int.Parse(ConfigMeneger.Configth.GetAnswer.Remove((ConfigMeneger.Configth.GetAnswer.Split(')')[0] + ")" + " ").Length, ConfigMeneger.Configth.GetAnswer.Length));

                                    UpdateUser(Convert.ToInt32(id.Split(' ')[1]), numPoint.ToString(), bot);


                                    bot.Messages.Send(new MessagesSendParams() { UserId = (long)Convert.ToInt64(id.Split(' ')[0]), Message = $"Ты получи {numPoint}", RandomId = new Random().Next() });
                                    Logger.Log($"[{DateTime.Now}][Answer from {((long)Convert.ToInt64(id.Split(' ')[0])).ToString()}]: you got {numPoint}");

                                    return true;
                                }
                            }
                        }
                        else if(ConfigMeneger.Configth.GetAnswer == answer)
                        {
                            foreach (var id in GetAllUserId(bot))
                            {
                                if (userId == (long)Convert.ToInt64(id.Split(' ')[0]))
                                {
                                    bot.Messages.Send(new MessagesSendParams() { UserId = (long)Convert.ToInt64(id.Split(' ')[0]), Message = $"Ты молодец", RandomId = new Random().Next() });
                                    Logger.Log($"[{DateTime.Now}][Answer from {((long)Convert.ToInt64(id.Split(' ')[0])).ToString()}]: You good");

                                    return true;
                                }
                            }

                            
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(send message Answer)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(send message Answer)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(send message Answer)]: {ex.Message}", bot);
                return false;
            }
        }

        private static void Send()
        {
            while (isActivate)
            {
                try
                {
                    if (ConfigMeneger.Configth.ActivateMessageSend == true)
                    {
                        if (DateTime.Now.Hour == ConfigMeneger.Configth.DateTime.Hour)
                        {
                            if (DateTime.Now.Minute == ConfigMeneger.Configth.DateTime.Minute)
                            {
                                if (DateTime.Now.Second == ConfigMeneger.Configth.DateTime.Second)
                                {
                                    foreach (var Users in GetAllUserId(bot))
                                    {
                                        bot.Messages.Send(new MessagesSendParams() { UserId = (long)Convert.ToInt64(Users.Split(' ')[0]), Message = ConfigMeneger.Configth.Message, RandomId = new Random().Next() });
                                        Logger.Log($"[{DateTime.Now}][Answer from {((long)Convert.ToInt64(Users.Split(' ')[0])).ToString()}]: {ConfigMeneger.Configth.Message}");
                                    }
                                }
                            }
                        }

                        if (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 49)
                        {
                            if (DateTime.Now.Hour + 23 == ConfigMeneger.Configth.DateTime.Hour)
                            {
                                if (DateTime.Now.Minute + 50 == ConfigMeneger.Configth.DateTime.Minute)
                                {
                                    if (DateTime.Now.Second == ConfigMeneger.Configth.DateTime.Second)
                                    {
                                        foreach (var Users in GetAllUserId(bot))
                                        {
                                            bot.Messages.Send(new MessagesSendParams() { UserId = (long)Convert.ToInt64(Users.Split(' ')[0]), Message = ConfigMeneger.Configth.Message, RandomId = new Random().Next() });
                                            Logger.Log($"[{DateTime.Now}][Answer from {((long)Convert.ToInt64(Users.Split(' ')[0])).ToString()}]: {ConfigMeneger.Configth.Message}");
                                        }
                                    }
                                }
                            }
                        }
                        else if (DateTime.Now.Hour == 23)
                        {
                            if (DateTime.Now.Hour + 23 == ConfigMeneger.Configth.DateTime.Hour)
                            {
                                if (DateTime.Now.Minute + 10 == ConfigMeneger.Configth.DateTime.Minute)
                                {
                                    if (DateTime.Now.Second == ConfigMeneger.Configth.DateTime.Second)
                                    {
                                        foreach (var Users in GetAllUserId(bot))
                                        {
                                            bot.Messages.Send(new MessagesSendParams() { UserId = (long)Convert.ToInt64(Users.Split(' ')[0]), Message = ConfigMeneger.Configth.Message, RandomId = new Random().Next() });
                                            Logger.Log($"[{DateTime.Now}][Answer from {((long)Convert.ToInt64(Users.Split(' ')[0])).ToString()}]: {ConfigMeneger.Configth.Message}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now}][exception(send message Send)]: {ex.Message}");
                    Console.ResetColor();
                    Logger.Log($"[{DateTime.Now}][exception(send message Send)]: {ex.Message}");
                    ExceptionMove.Exception($"[{DateTime.Now}][exception(send message Send)]: {ex.Message}", bot);
                }
            }
        }

        private static CookieContainer Auth()
        {
            try
            {
                CookieContainer cookieContainer = new CookieContainer(10000, 10000, 10000);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/private/api/auth.php?USER_LOGIN={ConfigMeneger.Configth.Login}&USER_HASH={ConfigMeneger.Configth.Hash}&type=json");
                request.CookieContainer = cookieContainer;
                request.Method = "POST";

                WebResponse response = request.GetResponse();

                using (Stream stream = response.GetResponseStream())
                {
                    string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                response.Close();

                Logger.Log($"[{DateTime.Now}][exception(command(Auth))]: Authorization completed");

                return request.CookieContainer;
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command(Auth))]: Error {ex.Message}"); return null; }
        }

        private static string NeedParams(int j, long idParams, dynamic json, VkApi bot)
        {
            dynamic cust = json["_embedded"]["items"][j]["custom_fields"];

            try
            {
                foreach (var item in cust)
                {
                    if (item["id"] == idParams.ToString())
                    {
                        return item["values"][0]["value"];
                    }
                }

                return " ";
            }
            catch (Exception ex)
            {
                Logger.Log($"[{DateTime.Now}]: error {ex.Message}(user not found)");
                ExceptionMove.Exception($"[{DateTime.Now}]: error {ex.Message}(user not found)", bot);
                return " ";
            }
        }

        private static string[] GetAllUserId(VkApi bot)
        {
            try
            {
                HttpWebRequest request_get = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");
                request_get.Method = "GET";
                request_get.CookieContainer = Auth();

                WebResponse response_get = request_get.GetResponse();

                List<string> output = new List<string>();

                using (Stream stream = response_get.GetResponseStream())
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(new StreamReader(response_get.GetResponseStream()).ReadToEnd());

                    for (int i = 0; i < json["_embedded"]["items"].Count; i++)
                    {
                        output.Add(((long)Convert.ToInt64(NeedParams(i, ConfigMeneger.Configth.IdUserId, json, bot))).ToString() + " " + ((long)Convert.ToInt64(json["_embedded"]["items"][i]["id"])).ToString());
                    }
                }

                request_get.Abort();
                response_get.Close();

                Logger.Log($"[{DateTime.Now}][exception(command (GetAllUserId))]: {output}");
                return output.ToArray();
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command (GetAllUserId))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command (GetAllUserId))]: {ex.Message}", bot); return null; }
        }

        private static int UpdateUser(int number, string parameters, VkApi bot)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");

                const char kav = '"';

                string strRequest = "{" + kav + "update" + kav + ":[{" + kav + "id" + kav + ":" + number + "," + kav + "updated_at" + kav + ":" + kav + UpdateAt(number, bot) + kav + "," + kav + "custom_fields" + kav + ":[" + "{" + kav + "id" + kav + ":" + kav;

                strRequest += ConfigMeneger.Configth.IdPoints.ToString();

                strRequest += kav + "," + kav + "values" + kav + ":[{" + kav + "value" + kav + ":" + kav + parameters + kav + "}]}]}]}";

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = Encoding.UTF8.GetBytes(strRequest).Length;
                request.CookieContainer = Auth();

                request.GetRequestStream().Write(Encoding.UTF8.GetBytes(strRequest), 0, Encoding.UTF8.GetBytes(strRequest).Length);

                WebResponse response = request.GetResponse();

                string responseString;

                using (Stream stream = response.GetResponseStream())
                {
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                Logger.Log($"[{DateTime.Now}][exception(command (UpdateUser))]: end({responseString})");

                return 0;
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command (UpdateUser))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command (UpdateUser))]: {ex.Message}", bot); return -1; }
        }

        private static string UpdateAt(long id, VkApi bot)
        {
            try
            {
                HttpWebRequest request_get = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts?id={id}");
                request_get.Method = "GET";
                request_get.CookieContainer = Auth();

                WebResponse response_get = request_get.GetResponse();

                using (Stream stream = response_get.GetResponseStream())
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(new StreamReader(response_get.GetResponseStream()).ReadToEnd());

                    return json["_embedded"]["items"][0]["updated_at"];
                }
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command (UpdateUser(UpdateAt)))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command (UpdateUser(UpdateAt)))]: {ex.Message}", bot); return " "; }
        }
    }
}
