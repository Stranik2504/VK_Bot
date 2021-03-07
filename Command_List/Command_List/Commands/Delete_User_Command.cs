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
    public class Delete_User_Command : Point_Command
    {
        public override string[] NameCommand => List("/delete", "/del", "/удалить");

        public override string NameClass => "Команда для удаления юзера из базы";

        public override string Explanation => "/delete";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 1)
            {
                string messageString = GetAllPoints(bot);

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Список людей в базе данных(Команда для удаления юзера из базы: /delete {id}): \n" + messageString, RandomId = new Random().Next() });

                return $"List of people in datebase(Command for deleting a user: /delete [id]): \n + {messageString}";
            }
            else
            {
                int numberUser = Convert.ToInt32(message.Text.Split(' ')[1]);

                if (DeleteUser(numberUser, bot) == 0)
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Юзер удален", RandomId = new Random().Next() });

                    return "User deleted";
                }
                else
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Ошибка, при попытке удаления юзера", RandomId = new Random().Next() });

                    return "Error to delete user";
                }
            }
        }

        private int DeleteUser(int numberUser, VkApi bot)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");

                const char kav = '"';

                string strRequest = "{" + kav + "delete" + kav + ":[" + numberUser + "]}";

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = Encoding.UTF8.GetBytes(strRequest).Length;
                request.CookieContainer = Auth();

                request.GetRequestStream().Write(Encoding.UTF8.GetBytes(strRequest), 0, Encoding.UTF8.GetBytes(strRequest).Length);

                WebResponse response = request.GetResponse();

                string responseString = "";

                using (Stream stream = response.GetResponseStream())
                {
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }

                return 0;
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(DeleteUser))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(DeleteUser))]: {ex.Message}", bot); return -1; }
        }
    }
}
