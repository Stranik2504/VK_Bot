using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Message_Command : Point_Command
    {
        public override string[] NameCommand => List("/message", "/мессадж", "/сообщени");

        public override string NameClass => "Команда для отправки сообщения юзеру";

        public override string Explanation => "/message {User id} {Cообщение}";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            if ((numberAccess <= Convert.ToInt32(Access)) && (numberAccess >= 0))
            {
                if (message.Text.Split(' ').Length >= 2)
                {
                    if (message.Text.Split(' ')[1] != null && message.Text.Split(' ')[1] != "" && message.Text.Split(' ')[1] != " ")
                    {
                        if (message.Text.Split(' ')[1].ToLower() == "all" && (message.Text.Split(' ')[1].ToLower() + " " + message.Text.Split(' ')[2].ToLower() != "all peoples"))
                        {
                            string text = message.Text.Remove(0, (message.Text.Split(' ')[0] + "  " + message.Text.Split(' ')[1]).Length);

                            foreach (var userid in GetAllUserId(bot))
                            {
                                bot.Messages.Send(new MessagesSendParams() { UserId = userid, Message = text, RandomId = new Random().Next() });
                            }

                            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Сообщение было отправлено пользователям; сообщение = {text}", RandomId = new Random().Next() });

                            return $"The message was sent to users; message = {text}";
                        }
                        else if (message.Text.Split(' ')[1].ToLower() + " " + message.Text.Split(' ')[2].ToLower() == "all peoples")
                        {
                            if (PeopleList.Peoples.Count > 0)
                            {
                                string text = message.Text.Remove(0, (message.Text.Split(' ')[0] + "   " + message.Text.Split(' ')[1] + message.Text.Split(' ')[2]).Length);

                                foreach (var people in PeopleList.Peoples)
                                {
                                    bot.Messages.Send(new MessagesSendParams() { UserId = people.UserId, Message = text, RandomId = new Random().Next() });
                                }

                                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Сообщение было отправлено пользователям; сообщение = {text}", RandomId = new Random().Next() });

                                return $"The message was sent to users; message = {text}";
                            }
                            else
                            {
                                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Нету юзеров", RandomId = new Random().Next() });

                                return "Users not found";
                            }
                        }
                        else
                        {
                            int.TryParse(message.Text.Split(' ')[1], out int UserId);
                            string text = message.Text.Remove(0, (message.Text.Split(' ')[0] + "  " + message.Text.Split(' ')[1]).Length);

                            bot.Messages.Send(new MessagesSendParams() { UserId = UserId, Message = text, RandomId = new Random().Next() });

                            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Сообщение было отправлено пользователю с id: {UserId}; сообщение = {text}", RandomId = new Random().Next() });

                            return $"The message was sent to user with id: {UserId}; message = {text}";
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
            else
            {
                return $"Error: Data entered incorrectly(This user {message.PeerId.Value} connot using this command)";
            }
        }
    }
}
