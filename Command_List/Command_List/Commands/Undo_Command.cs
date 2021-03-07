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
    class Undo_Command : Admin_Command
    {
        public override string[] NameCommand => List("/undo", "/отменить");

        public override string NameClass => "Команда для отмены заявки из списка";

        public override string Explanation => "/undo {Номер вашей заявки(Если у вас несколько заявок)}";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            bool cheak = false;
            int count = 0;

            for (int i = 0; i < RegisterList.Users.Count; i++)
            {
                if (RegisterList.Users[i].UserId == message.PeerId.Value)
                {
                    if (message.Text.Split(' ').Length == 1)
                    {
                        RegisterList.Users.RemoveAt(i);
                        cheak = true;
                        break;
                    }
                    else
                    {
                        count++;
                    }

                    if (count == Convert.ToInt32(message.Text.Split(' ')[1]))
                    {
                        RegisterList.Users.RemoveAt(i);
                        cheak = true;
                        break;
                    }
                }
            }

            if (cheak == true)
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Заявка удалена", RandomId = new Random().Next() });

                return "Application removed";
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "У вас нет активных заявок", RandomId = new Random().Next() });

                return "You don't have application";
            }
        }
    }
}
