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
    public class Registration_Command : Admin_Command
    {
        public override string[] NameCommand => List("/register", "/registration", "/reg", "/регистрация", "/рег", "register", "registration", "reg", "регистрация", "рег");

        public override string NameClass => "Команда для регистрации";

        public override string Explanation => "/register {Ваше имя}";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 1)
            {
                if ((numberAccess >= 0) && (numberAccess < Convert.ToInt32(Access)))
                {
                    string adminExplanation = "/register {User id} {Points} {Name user}";

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = adminExplanation, RandomId = new Random().Next() });

                    return adminExplanation;
                }
                else
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = Explanation, RandomId = new Random().Next() });

                    return Explanation;
                }
            }
            else if ((numberAccess >= 0) && (numberAccess < Convert.ToInt32(Access)))
            {
                try
                {
                    RegisterList.Users.Add(new Classes.User() { UserId = (long)Convert.ToDouble(message.Text.Split(' ')[1]), Name = message.Text.Remove(0, (message.Text.Split(' ')[0] + " " + message.Text.Split(' ')[1] + " " + message.Text.Split(' ')[2] + " ").Length), Points = Convert.ToInt32(message.Text.Split(' ')[2]) });

                    RegisterList.SaveListUser();

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Юзер с id {message.Text.Split(' ')[1]}({message.PeerId.Value}) и с количеством поинтов = {message.Text.Split(' ')[2]} добавлен в очередь", RandomId = new Random().Next() });

                    return $"User with id {message.Text.Split(' ')[1]}({message.PeerId.Value}) and points = {message.Text.Split(' ')[2]} added in turn";
                }
                catch
                {
                    RegisterList.Users.Add(new Classes.User() { UserId = message.PeerId.Value, Name = message.Text.Remove(0, (message.Text.Split(' ')[0] + " ").Length), Points = 0 });

                    RegisterList.SaveListUser();

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Юзер с id {message.Text.Split(' ')[1]}({message.PeerId.Value}) добавлен в очередь", RandomId = new Random().Next() });

                    return $"User with id {message.Text.Split(' ')[1]}({message.PeerId.Value}) added in turn";
                }
            }
            else
            {
                RegisterList.Users.Add(new Classes.User() { UserId = message.PeerId.Value, Name = message.Text.Remove(0, (message.Text.Split(' ')[0] + " ").Length), Points = 0 });

                RegisterList.SaveListUser();

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Вы добавлены в список, для добавления в базу", RandomId = new Random().Next() });

                return $"User with id {message.Text.Split(' ')[1]}({message.PeerId.Value}) added in turn";
            }
        }
    }
}
