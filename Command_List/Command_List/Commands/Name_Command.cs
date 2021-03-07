using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Name_Command : Command
    {
        public override string[] NameCommand => List("/name", "/имя", "имя", "name");

        public override string NameClass => "Имя бота";

        public override string Explanation => "/name";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Моё имя Аркадий", RandomId = new Random().Next() });

            return "Моё имя Аркадий";
        }
    }
}
