using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Spawn_Command : Admin_Command
    {
        public override string[] NameCommand => List("/spamm", "/spam", "/с", "/спам");

        public override string NameClass => "Команда для спама юзеру";

        public override string Explanation => "/spam {Количество} {User id} {Сообщение}";

        public override Access Access => Access.SuperAdmin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length >= 3)
            {
                if (message.Text.Split(' ')[1] != null && message.Text.Split(' ')[1] != "" && message.Text.Split(' ')[1] != " " && message.Text.Split(' ')[2] != null && message.Text.Split(' ')[2] != "" && message.Text.Split(' ')[2] != " ")
                {
                    int.TryParse(message.Text.Split(' ')[1], out int Count);

                    if ((Access)CheakAccess(message.PeerId.Value, bot) == Access.SuperAdmin)
                    {
                        if (Count > 100) { Count = 100; }
                    }

                    int.TryParse(message.Text.Split(' ')[2], out int UserId);
                    string text = message.Text.Remove(0, (message.Text.Split(' ')[0] + "   " + message.Text.Split(' ')[1] + message.Text.Split(' ')[2]).Length);

                    for (int i = 0; i < Count; i++)
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = UserId, Message = text, RandomId = new Random().Next() });
                    }

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Сообщение было отправлено пользователю с id: {UserId}; Сообщением = {text}; количеством = {Count}", RandomId = new Random().Next() });

                    return $"The message was sent to user with id: {UserId}; message = {text}; count = {Count}";
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
