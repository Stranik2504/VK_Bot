using System;
using System.Collections.Generic;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Triger_Command : Admin_Command
    {
        public override string[] NameCommand => List("/triger", "/tr", "/тригер", "/триг");

        public override string NameClass => "Команда для выполнения команды со стороны юзера";

        public override string Explanation => "/triger {User id} {Название команды}";

        public override Access Access => Access.SuperAdmin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length >= 3)
            {
                if (message.Text.Split(' ')[1] != null && message.Text.Split(' ')[1] != "" && message.Text.Split(' ')[1] != " ")
                {
                    int.TryParse(message.Text.Split(' ')[1], out int UserId);

                    string command = message.Text.Remove(0, (message.Text.Split(' ')[0] + "  " + message.Text.Split(' ')[1]).Length);

                    bool activate = false;
                    long MyId = message.PeerId.Value;
                    int UserAccess = CheakAccess(UserId, bot);

                    if (numberAccess <= UserAccess)
                    {
                        for (int i = 0; i < GetCommand.GetCommands().Count; i++)
                        {
                            for (int j = 0; j < GetCommand.GetCommands()[i].NameCommand.Length; j++)
                            {
                                if (GetCommand.GetCommands()[i].NameCommand[j] == command.Split(' ')[0])
                                {
                                    message.Text = message.Text.Remove(0, (message.Text.Split(' ')[0] + "  " + message.Text.Split(' ')[1]).Length);
                                    message.PeerId = UserId;
                                    GetCommand.GetCommands()[i].Execute(message, bot);
                                    activate = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (activate == true)
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = MyId, Message = $"Триггер был активирован для пользователя с id: {message.PeerId.Value}; Команда = {command}", RandomId = new Random().Next() });

                        return $"The triger was activate to user with id: {message.PeerId.Value}; command = {command}";
                    }
                    else
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = MyId, Message = $"Ошибка при активации команды({command}) у юзера с id: {message.PeerId.Value}", RandomId = new Random().Next() });

                        return $"Error to trigering command({command}) to user with id: {message.PeerId.Value}";
                    }
                }
                else
                {
                    return "Error";
                }
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = Explanation, RandomId = new Random().Next() });

                return Explanation;
            }
        }
    }
}
