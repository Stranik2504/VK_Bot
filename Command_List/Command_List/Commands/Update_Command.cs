using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Command_List.Commands
{
    public class Update_Command : Point_Command
    {
        public override string[] NameCommand => List("/update", "/up", "/обновить");

        public override string NameClass => "Команда для обновлений данных в базе";

        public override string Explanation => "/update";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 1)
            {
                string messageString = GetAllPoints(bot);

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Список людей находящихся в базе(Команда для обновления данных юзера: /update {id} {1 - user id; 2 - " + ConfigMeneger.Configth.NamePoints + "; 3 - промокод; 4 - скидка} {значения параметра}): \n" + messageString, RandomId = new Random().Next() });

                return "List of people in datebase(Command for updating a user: /update [id] [1 - user id; 2 - points; 3 - promocode; 4 - discount] [value params]): \n + {messageString}";
            }
            else
            {
                int number = Convert.ToInt32(message.Text.Split(' ')[1]);
                int numberParams = Convert.ToInt32(message.Text.Split(' ')[2]);
                string parametrs = message.Text.Remove(0, (message.Text.Split(' ')[0] + message.Text.Split(' ')[1] + message.Text.Split(' ')[2] + "   ").Length);

                if (numberParams > 0 && numberParams < 5)
                {
                    if (UpdateUser(number, numberParams, message.Text.Split(' ')[message.Text.Split(' ').Length - 1], bot) == 0)
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Параметры юзера обновлены", RandomId = new Random().Next() });

                        return "User updated";
                    }
                    else
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Ошибка в обновлении данных юзера", RandomId = new Random().Next() });

                        return "Error to update user";
                    }
                }
                else
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Параметр не найден", RandomId = new Random().Next() });

                    return "Params not found";
                }
            }
        }

        private int UpdateUser(int number, int numberParams, string parameters, VkApi bot)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");

                const char kav = '"';

                string strRequest = "{" + kav + "update" + kav + ":[{" + kav + "id" + kav + ":" + number + "," + kav + "updated_at" + kav + ":" + kav + UpdateAt(number, bot) + kav + "," + kav + "custom_fields" + kav + ":[" + "{" + kav + "id" + kav + ":" + kav;

                switch (numberParams)
                {
                    case 1:
                        strRequest += ConfigMeneger.Configth.IdUserId.ToString();
                        break;
                    case 2:
                        strRequest += ConfigMeneger.Configth.IdPoints.ToString();
                        break;
                    case 3:
                        strRequest += ConfigMeneger.Configth.IdPromocode.ToString();
                        break;
                    case 4:
                        strRequest += ConfigMeneger.Configth.IdDiscount.ToString();
                        break;
                }

                strRequest += kav + "," + kav + "values" + kav + ":[{" + kav + "value" + kav + ":" + kav + parameters + kav + "}]}]}]}";

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = Encoding.UTF8.GetBytes(strRequest).Length;
                request.CookieContainer = Auth();

                request.GetRequestStream().Write(Encoding.UTF8.GetBytes(strRequest), 0, Encoding.UTF8.GetBytes(strRequest).Length);

                WebResponse response =  request.GetResponse();

                string responseString;

                using (Stream stream = response.GetResponseStream())
                {
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(UpdateUser))]: end({responseString})");

                return 0;
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(UpdateUser))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(UpdateUser))]: {ex.Message}", bot); return -1; }
        }

        private string UpdateAt(long id, VkApi bot)
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
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(UpdateUser(UpdateAt)))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(UpdateUser(UpdateAt)))]: {ex.Message}", bot); return " "; }
        }
    }
}
