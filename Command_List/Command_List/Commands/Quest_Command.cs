using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Quest_Command : Admin_Command
    {
        public override string[] NameCommand => List("/quest", "/quests", "/квест");

        public override string NameClass => "Команда для просмотра числа накопленных юзерами";

        public override string Explanation => "/quests {User id}";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 1)
            {
                People[] peoples = PeopleList.Peoples.ToArray();

                for (int i = 0; i < peoples.Length; i++)
                {
                    for (int j = 0; j < peoples.Length - i - 1; j++)
                    {
                        if (peoples[j].CorrectAnswer < peoples[j + 1].CorrectAnswer)
                        {
                            People people = peoples[j];
                            peoples[j] = peoples[j + 1];
                            peoples[j + 1] = people;
                        }
                        else if (peoples[j].CorrectAnswer == peoples[j + 1].CorrectAnswer)
                        {
                            if (peoples[j].TimeEnd > peoples[j + 1].TimeEnd)
                            {
                                People people = peoples[j];
                                peoples[j] = peoples[j + 1];
                                peoples[j + 1] = people;
                            }
                        }
                    }
                }

                string output = "";
                int number = 1;

                foreach (var people in peoples)
                {
                    output += $"{number})Имя: {people.Name}(UserId:{people.UserId}); Номер вопроса на ответ: {people.NumberQuestions}; Количество ответов {people.CorrectAnswer}; \n";
                    number++;
                }

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = output, RandomId = new Random().Next() });

                return output;

            }
            else
            {
                if (message.Text.Split(' ')[1].ToLower() == "all")
                {
                    ConfigMeneger.Configth.IsWorkingQuest = false;
                    ConfigMeneger.SaveConfigth();

                    if (PeopleList.Peoples.Count > 0)
                    {
                        People[] peoples = PeopleList.Peoples.ToArray();

                        for (int i = 0; i < peoples.Length; i++)
                        {
                            for (int j = 0; j < peoples.Length - i - 1; j++)
                            {
                                if (peoples[j].CorrectAnswer < peoples[j + 1].CorrectAnswer)
                                {
                                    People people = peoples[j];
                                    peoples[j] = peoples[j + 1];
                                    peoples[j + 1] = people;
                                }
                                else if (peoples[j].CorrectAnswer == peoples[j + 1].CorrectAnswer)
                                {
                                    if (peoples[j].TimeEnd > peoples[j + 1].TimeEnd)
                                    {
                                        People people = peoples[j];
                                        peoples[j] = peoples[j + 1];
                                        peoples[j + 1] = people;
                                    }
                                }
                            }
                        }

                        string output = "";
                        int number = 1;

                        foreach (var people in peoples)
                        {
                            bot.Messages.Send(new MessagesSendParams() { UserId = people.UserId, Message = $"Вы заняли {number} место", RandomId = new Random().Next() });

                            output += $"{number})Имя: {people.Name}(UserId:{people.UserId}); Номер вопроса на ответ: {people.NumberQuestions}; Количество ответов {people.CorrectAnswer}; \n";
                            number++;
                        }

                        bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = output, RandomId = new Random().Next() });

                        return output;
                    }

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Окончание квеста", RandomId = new Random().Next() });
                    return "Quest ending";
                }
                else if (message.Text.Split(' ')[1].ToLower() == "start")
                {
                    ConfigMeneger.Configth.IsRegistretingQuest = true;
                    ConfigMeneger.SaveConfigth();

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Регистрация начата", RandomId = new Random().Next() });

                    return "Registration start";
                }
                else if (message.Text.Split(' ')[1].ToLower() == "end")
                {
                    ConfigMeneger.Configth.IsRegistretingQuest = false;
                    ConfigMeneger.SaveConfigth();

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Регистрация закончена", RandomId = new Random().Next() });

                    return "Registration end";
                }
                else
                {
                    foreach (var people in PeopleList.Peoples)
                    {
                        if (message.Text.Split(' ')[1] == people.UserId.ToString() || message.Text.Split(' ')[1] == people.Name)
                        {
                            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Имя: {people.Name}(UserId:{people.UserId}); Номер вопроса на ответ: {people.NumberQuestions}; Количество ответов {people.CorrectAnswer};", RandomId = new Random().Next() });

                            return $"Имя: {people.Name}(UserId:{people.UserId}); Номер вопроса на ответ: {people.NumberQuestions}; Количество ответов {people.CorrectAnswer};";
                        }
                    }

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Юзер не найден", RandomId = new Random().Next() });

                    return "User not found";
                }
            }
        }
    }
}
