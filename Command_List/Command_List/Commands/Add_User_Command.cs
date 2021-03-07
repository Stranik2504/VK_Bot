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
    public class Add_User_Command : Point_Command
    {
        public override string[] NameCommand => List("/add", "/добавить");

        public override string NameClass => "Команда для добавления юзеров в базу";

        public override string Explanation => $"/add";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 1)
            {
                string messageString = "";
                int number = 0;

                foreach (var user in RegisterList.Users)
                {
                    messageString += $"[{number}]{user.Name}({user.UserId}) \n";
                    number++;
                }

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Список людей, которых нужно добавить в базу данных(Команда для добавления юзера: /add {номер}): \n" + messageString, RandomId = new Random().Next() });

                return $"List of people to add to the database(Сommand for adding a user: /add [number]): \n + {messageString}";
            }
            else if (message.Text.Split(' ').Length == 2)
            {
                int numberUser = Convert.ToInt32(message.Text.Split(' ')[1]);

                if (AddUser(RegisterList.Users[numberUser], bot) == 0)
                {
                    try
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = RegisterList.Users[numberUser].UserId, Message = "Ты добавлен", RandomId = new Random().Next() });

                        return "You added";
                    }
                    catch (Exception ex)
                    {
                        ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}", bot);
                        Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}");
                    }

                    RegisterList.Users.RemoveAt(numberUser);

                    RegisterList.SaveListUser();

                    ConfigMeneger.SaveConfigth();


                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Юзер добавлен", RandomId = new Random().Next() });

                    return $"User added";
                }
                else
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Ошибка в добавлении юзера в базу", RandomId = new Random().Next() });

                    return $"Error to add user";
                }
            }
            else
            {
                var user = new Classes.User() { UserId = (long)Convert.ToDouble(message.Text.Split(' ')[1]), Name = message.Text.Remove(0, (message.Text.Split(' ')[0] + " " + message.Text.Split(' ')[1] + " " + message.Text.Split(' ')[2] + " ").Length), Points = Convert.ToInt32(message.Text.Split(' ')[2]) };

                AddUser(user, bot);

                ConfigMeneger.SaveConfigth();

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Юзер добавлен", RandomId = new Random().Next() });

                return "User added";
            }
        }

        private int AddUser(Classes.User user, VkApi bot)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"https://{ConfigMeneger.Configth.Account}.amocrm.ru/api/v2/contacts");

                const char kav = '"';

                string strRequest;

                strRequest = "{" + kav + "add" + kav + ":[{" + kav + "name" + kav + ":" + kav + user.Name + kav + "," + kav + "custom_fields" + kav + ":[" + "{" + kav + "id" + kav + ":" + kav + ConfigMeneger.Configth.IdUserId.ToString() + kav + "," + kav + "values" + kav + ":[{" + kav + "value" + kav + ":" + kav + user.UserId + kav + "}]}," + "{" + kav + "id" + kav + ":" + kav + ConfigMeneger.Configth.IdPoints + kav + "," + kav + "values" + kav + ":[{" + kav + "value" + kav + ":" + kav + user.Points + kav + "}]}]}]}";

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = Encoding.UTF8.GetBytes(strRequest).Length;
                request.CookieContainer = Auth();

                request.GetRequestStream().Write(Encoding.UTF8.GetBytes(strRequest), 0, Encoding.UTF8.GetBytes(strRequest).Length);

                request.GetResponse();

                return 0;
            }
            catch (Exception ex) { Logger.Log($"[{DateTime.Now}][exception(command {NameClass}(AddUser))]: {ex.Message}"); ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass}(AddUser))]: {ex.Message}", bot); return -1; }
        }
    }
}
