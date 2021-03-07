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
    public class Remove_Command : Admin_Command
    {
        public override string[] NameCommand => List("/remove", "/убрать");

        public override string NameClass => "Команда для удаления заявок";

        public override string Explanation => "/remove";

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

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Список людей, которых нужно убрать из очереди(Команда для удаления юзера из очереди:/remove {number}): \n" + messageString, RandomId = new Random().Next() });

                return $"List of people to remove from turn(Command for removing user from datebase:/remove [number]): \n + {messageString}";
            }
            else
            {
                if (RegisterList.Users.Count > 0)
                {
                    int numberUser = Convert.ToInt32(message.Text.Split(' ')[1]);

                    RegisterList.Users.RemoveAt(numberUser);

                    RegisterList.SaveListUser();

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Юзер удален из очереди", RandomId = new Random().Next() });

                    return "User removed";
                }
                else
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "такого номера не существует", RandomId = new Random().Next() });

                    return "this nomber not found";
                }
            }
        }
    }
}
