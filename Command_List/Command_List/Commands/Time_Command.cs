using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Time_Command : Command
    {
        public override string[] NameCommand => List("/time", "/время", "time", "время");

        public override string NameClass => "Узнать время";

        public override string Explanation => "/time";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Время: " + DateTime.Now.ToString(), RandomId = new Random().Next() });

            return DateTime.Now.ToString();
        }
    }
}
