using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Strart_Command : Command
    {
        public override string[] NameCommand => List("/start", "/myname", "/моеимя", "начать", "/начать");

        public override string NameClass => "Команда для регистрации на квест";

        public override string Explanation => "/myname {Ваше имя}";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            if (ConfigMeneger.Configth.IsRegistretingQuest == true)
            {
                if (message.Text.ToLower() == "/start" || message.Text.ToLower() == "/начать" || message.Text.ToLower() == "начать")
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Введите команду /myname и, через пробел, вашу фамилию и имя", RandomId = new Random().Next() });

                    return "Enter the /myname command, and your last name and first name separated by a space";
                }
                else
                {
                    if (message.Text.Split(' ').Length > 2)
                    {
                        string Name = message.Text.Remove(0, (message.Text.Split(' ')[0] + " ").Length);

                        PeopleList.Peoples.Add(new People() { Name = Name, UserId = message.PeerId.Value, NumberQuestions = -1, CorrectAnswer = 0 });

                        PeopleList.SavePeople();

                        bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Вы зарегистрированы. Для ответа на вопросы введите /answer и через пробел ответ", RandomId = new Random().Next() });

                        return "You registred. To answer questions, enter /answer and space-separated answer";
                    }
                    else
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Ошибка ввода", RandomId = new Random().Next() });

                        return "Error input";
                    }
                }
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Квест не активен", RandomId = new Random().Next() });

                return "Quest not anable";
            }
        }
    }
}
