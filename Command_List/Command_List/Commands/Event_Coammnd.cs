using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Event_Coammnd : Command
    {
        public override string[] NameCommand => List("/event", "/евент", "/событие");

        public override string NameClass => "Команда получения событий";

        public override string Explanation => "/event";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = DateTime.Now.ToString(), RandomId = new Random().Next() });

            return DateTime.Now.ToString();
        }
    }
}
