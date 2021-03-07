using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class GetCommands_Command : Admin_Command
    {
        public override string[] NameCommand => List("/command", "/help", "/commands", "command", "commands", "/команды", "/команда", "/помощь", "команды", "команда", "помощь");

        public override string NameClass => "Команда для получения всех команд";

        public override string Explanation => "/command";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            int count = 1;
            string answer = "";

            if (message.Text.Split(' ').Length > 1)
            {
                if (message.Text.Split(' ')[1] == "all")
                {
                    foreach (var command in GetCommand.GetCommands())
                    {
                        if (command.Access >= (Access)numberAccess)
                        {
                            answer += $"{count}. {command.NameClass} - {command.NameCommand[0]} (";
                            for (int i = 1; i < command.NameCommand.Length; i++)
                            {
                                if (i + 1 < command.NameCommand.Length) { answer += command.NameCommand[i] + ", "; }
                                else { answer += command.NameCommand[i] + ") \n"; }
                            }
                            count++;
                        }
                    }
                }
            }
            else
            {
                foreach (var command in GetCommand.GetCommands())
                {
                    if (command.Access >= (Access)numberAccess)
                    {
                        answer += $"{count}. {command.NameClass} - {command.NameCommand[0]} \n";
                        count++;
                    }
                }
            }

            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = answer, RandomId = new Random().Next() });

            return answer;
        }
    }
}
