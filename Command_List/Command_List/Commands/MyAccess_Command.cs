using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Command_List.Commands
{
    public class MyAccess_Command : Admin_Command
    {
        public override string[] NameCommand => List("/access", "/myaccess", "/доступ", "/мойдоступ");

        public override string NameClass => "Команда для получения уровня доступа";

        public override string Explanation => "/access";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = ((Access)numberAccess).ToString(), RandomId = new Random().Next() });

            return ((Access)numberAccess).ToString();
        }
    }
}
