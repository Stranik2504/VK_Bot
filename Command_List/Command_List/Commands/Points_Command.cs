using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Command_List.Commands
{
    public class Points_Command : Point_Command
    {
        public override string[] NameCommand => List("/points", "/point", "/поинты", "/очки", "/поинт", "point", "points", "поинты", "поинт");

        public override string NameClass => $"Команда для получения информации по количеству {ConfigMeneger.Configth.NamePoints}";

        public override string Explanation => "/points";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            string pointsMessage = Points(message.Text, message.PeerId.Value, CheakAccess(message.PeerId.Value, bot), bot);

            if (pointsMessage != "-1")
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = pointsMessage, RandomId = new Random().Next() });

                return pointsMessage;
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Ошибка в проверке {ConfigMeneger.Configth.NamePoints}", RandomId = new Random().Next() });

                return $"Error to check {ConfigMeneger.Configth.NamePoints}";
            }
        }

        private string Points(string message, long UserId, int MyAccess, VkApi bot)
        {
            if ((MyAccess == -1) || ((MyAccess <= Convert.ToInt32(Access)) && (MyAccess >= 0) && (message.Split(' ').Length == 1)))
            {
                try
                {
                    string point = GetPoints(UserId, bot);

                    if (point != "-1" && point != "-2") { return $"Ты имеешь {point} {ConfigMeneger.Configth.NamePoints}"; } else { return "Тебя нет в базе данных, чтобы зарегистрироваться введи команду /reg"; }
                }
                catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(Points(User)))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(Points(User)))]: {ex.Message}", bot); return "-1"; }
            }
            else if ((MyAccess < Convert.ToInt32(Access)) && (MyAccess >= 0) && (message.Split(' ').Length >= 2))
            {
                if ((message.Split(' ')[1] != "all") && (message.Split(' ').Length >= 2))
                {
                    try
                    {
                        string point = GetPoints(message.Remove(0, (message.Split(' ')[0] + " ").Length), bot);

                        if (point != "-1" && point != "-2") { return $"{point}"; } else if (point == "-2") { return "Ошибка"; } else { return "Юзера нет в базе, чтобы зарегистрировать введи команду /reg"; }
                    }
                    catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(Points(Admin)))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(Points(Admin)))]: {ex.Message}", bot); return "-1"; }
                }
                else if (message.Split(' ')[1] == "all")
                {
                    try
                    {
                        string point = GetAllPoints(bot);

                        if (point != "-1" && point != "-2") { return point; } else { return "Ошибка"; }
                    }
                    catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(Points(Admin(all))))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(Points(Admin(all))))]: {ex.Message}", bot); return "-1"; }
                }
            }

            return "-1";
        }

        private string GetPoints(long UserId, VkApi bot)
        {
            try
            {
                HttpWebRequest request_get = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");
                request_get.Method = "GET";
                request_get.CookieContainer = Auth();

                WebResponse response_get = request_get.GetResponse();

                using (Stream stream = response_get.GetResponseStream())
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(new StreamReader(response_get.GetResponseStream()).ReadToEnd());

                    if (json["_embedded"] != null)
                    {
                        if (json["_embedded"]["items"] != null)
                        {
                            for (int i = 0; i < json["_embedded"]["items"].Count; i++)
                            {
                                if (NeedParams(i, ConfigMeneger.Configth.IdUserId, json, bot) == UserId.ToString())
                                {
                                    Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetPoints))]: {NeedParams(i, ConfigMeneger.Configth.IdPoints, json, bot)}");
                                    return $"{NeedParams(i, ConfigMeneger.Configth.IdPoints, json, bot)}";
                                }
                            }
                        }
                    }
                }

                request_get.Abort();
                response_get.Close();

                return "-1";
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetPoints))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(GetPoints))]: {ex.Message}", bot); return "-2"; }
        }

        private string GetPoints(string Params, VkApi bot)
        {
            try
            {
                HttpWebRequest request_get = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");
                request_get.Method = "GET";
                request_get.CookieContainer = Auth();

                WebResponse response_get = request_get.GetResponse();

                using (Stream stream = response_get.GetResponseStream())
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(new StreamReader(response_get.GetResponseStream()).ReadToEnd());

                    if (json["_embedded"] != null)
                    {
                        if (json["_embedded"]["items"] != null)
                        {
                            for (int i = 0; i < json["_embedded"]["items"].Count; i++)
                            {
                                if (json["_embedded"]["items"][i]["name"].ToString() == Params || json["_embedded"]["items"][i]["id"].ToString() == Params)
                                {
                                    Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetPoints))]: [{json["_embedded"]["items"][i]["id"]}] Имя = {json["_embedded"]["items"][i]["name"]}(UserId = {NeedParams(i, ConfigMeneger.Configth.IdUserId, json, bot)}): {ConfigMeneger.Configth.NamePoints} = {NeedParams(i, ConfigMeneger.Configth.IdPoints, json, bot)}, Промокод = {NeedParams(i, ConfigMeneger.Configth.IdPromocode, json, bot)}, Скидка = {NeedParams(i, ConfigMeneger.Configth.IdDiscount, json, bot)}");
                                    return $"[{json["_embedded"]["items"][i]["id"]}]" + " Имя = " + json["_embedded"]["items"][i]["name"] + $"(UserId = {NeedParams(i, ConfigMeneger.Configth.IdUserId, json, bot)}): {ConfigMeneger.Configth.NamePoints} = {NeedParams(i, ConfigMeneger.Configth.IdPoints, json, bot)}, Промокод = {NeedParams(i, ConfigMeneger.Configth.IdPromocode, json, bot)}, Скидка = {NeedParams(i, ConfigMeneger.Configth.IdDiscount, json, bot)}";
                                }
                                else
                                {
                                    for (int j = 0; j < json["_embedded"]["items"][i]["custom_fields"].Count; j++)
                                    {
                                        if (json["_embedded"]["items"][i]["custom_fields"][j]["values"][0]["value"].ToString() == Params)
                                        {
                                            Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetPoints))]: [{json["_embedded"]["items"][i]["id"]}] Name = {json["_embedded"]["items"][i]["name"]}(UserId = {NeedParams(i, ConfigMeneger.Configth.IdUserId, json, bot)}): {ConfigMeneger.Configth.NamePoints} = {NeedParams(i, ConfigMeneger.Configth.IdPoints, json, bot)}, Промокод = {NeedParams(i, ConfigMeneger.Configth.IdPromocode, json, bot)}, Скидка = {NeedParams(i, ConfigMeneger.Configth.IdDiscount, json, bot)}");
                                            return $"[{json["_embedded"]["items"][i]["id"]}]" + " Имя = " + json["_embedded"]["items"][i]["name"] + $"(UserId = {NeedParams(i, ConfigMeneger.Configth.IdUserId, json, bot)}): {ConfigMeneger.Configth.NamePoints} = {NeedParams(i, ConfigMeneger.Configth.IdPoints, json, bot)}, Промокод = {NeedParams(i, ConfigMeneger.Configth.IdPromocode, json, bot)}, Скидка = {NeedParams(i, ConfigMeneger.Configth.IdDiscount, json, bot)}";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                request_get.Abort();
                response_get.Close();

                return "-1";
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(GetPoints))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(GetPoints))]: {ex.Message}", bot); return "-2"; }
        }
    }
}
