using System;
using Classes;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using VkNet;
using VkNet.Model;

namespace Command_List
{
    public abstract class Point_Command : Admin_Command
    {
        protected CookieContainer Auth()
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

                Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(Auth))]: Authorization completed");

                return request.CookieContainer;
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(Auth))]: Error {ex.Message}"); return null; }
        }

        protected string NeedParams(int j, long idParams, dynamic json, VkApi bot)
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
                Logger.Log($"[{DateTime.Now}][{NameCommand[0]}]: error {ex.Message}(user not found)");
                ExceptionMove.Exception($"[{DateTime.Now}][{NameCommand[0]}]: error {ex.Message}(user not found)", bot);
                return " ";
            }
        }

        protected string GetAllPoints(VkApi bot)
        {
            try
            {
                HttpWebRequest request_get = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");
                request_get.Method = "GET";
                request_get.CookieContainer = Auth();

                WebResponse response_get = request_get.GetResponse();

                string output = "";

                using (Stream stream = response_get.GetResponseStream())
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(new StreamReader(response_get.GetResponseStream()).ReadToEnd());

                    for (int i = 0; i < json["_embedded"]["items"].Count; i++)
                    {
                        output += $"[{json["_embedded"]["items"][i]["id"]}] Имя = {json["_embedded"]["items"][i]["name"]}(UserId = {NeedParams(i, ConfigMeneger.Configth.IdUserId, json, bot)}): {ConfigMeneger.Configth.NamePoints} = {NeedParams(i, ConfigMeneger.Configth.IdPoints, json, bot)}, Промокод = {NeedParams(i, ConfigMeneger.Configth.IdPromocode, json, bot)}, Скидка = {NeedParams(i, ConfigMeneger.Configth.IdDiscount, json, bot)}";

                        if (i != json["_embedded"]["items"].Count - 1) { output += "\n"; }
                    }
                }

                request_get.Abort();
                response_get.Close();

                Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetAllPoints))]: {output}");
                return output;
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetAllPoints))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(GetAllPoints))]: {ex.Message}", bot); return "-1"; }
        }

        protected long[] GetAllUserId(VkApi bot)
        {
            try
            {
                HttpWebRequest request_get = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");
                request_get.Method = "GET";
                request_get.CookieContainer = Auth();

                WebResponse response_get = request_get.GetResponse();

                List<long> output = new List<long>();

                using (Stream stream = response_get.GetResponseStream())
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(new StreamReader(response_get.GetResponseStream()).ReadToEnd());

                    for (int i = 0; i < json["_embedded"]["items"].Count; i++)
                    {
                        output.Add((long)Convert.ToInt64(NeedParams(i, ConfigMeneger.Configth.IdUserId, json, bot)));
                    }
                }

                request_get.Abort();
                response_get.Close();

                Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetAllUserId))]: {output}");
                return output.ToArray();
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetAllUserId))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(GetAllUserId))]: {ex.Message}", bot); return null; }
        }

        public override string Execute(Message message, VkApi bot)
        {
            try
            {
                if (message != null && bot != null && message.PeerId != null)
                {
                    CheakAccessAdmin(message.PeerId.Value, bot);

                    string output = Move(message, bot);

                    Logger.Log($"[{DateTime.Now}][Answer to {message.PeerId.Value}][{NameClass}]: {output}");
                    return $"[{DateTime.Now}][Answer to {message.PeerId.Value}][{NameClass}]: {output}";
                }
                else
                {
                    Logger.Log($"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error");
                    return $"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error";
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}", bot);
                return $"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}";
            }
        }
    }
}
