using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Quest_Start_Command : Admin_Command
    {
        public override string[] NameCommand => List("/quest_start");

        public override string NameClass => "Команда для начала квеста";

        public override string Explanation => "/quest_start";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            ConfigMeneger.Configth.IsWorkingQuest = true;
            ConfigMeneger.SaveConfigth();

            int number = -1;

            foreach (var people in PeopleList.Peoples)
            {
                people.NumberQuestions = 1;
                people.CorrectAnswer = 0;
                bot.Messages.Send(new MessagesSendParams() { UserId = people.UserId, Message = QuestionsList.Questions[0].Question, RandomId = new Random().Next() });

                number++;
            }

            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Квест начат", RandomId = new Random().Next() });

            DateTime timeStart = DateTime.Now;

            int plus = -1;
            if (timeStart.Hour + ConfigMeneger.Configth.TimeQuest.Hour >= 24) { plus++; }
            else if (timeStart.Hour + ConfigMeneger.Configth.TimeQuest.Hour == 23 && timeStart.Minute + ConfigMeneger.Configth.TimeQuest.Minute >= 60) { plus++; }
            else if (timeStart.Hour + ConfigMeneger.Configth.TimeQuest.Hour == 23 && timeStart.Minute + ConfigMeneger.Configth.TimeQuest.Minute == 59 && timeStart.Second + ConfigMeneger.Configth.TimeQuest.Second >= 60) { plus++; }

            while (((DateTime.Now.Second < (timeStart.Second + ConfigMeneger.Configth.TimeQuest.Second) % 60) || (DateTime.Now.Minute < (timeStart.Minute + ConfigMeneger.Configth.TimeQuest.Minute) % 60) || (DateTime.Now.Hour < (timeStart.Hour + ConfigMeneger.Configth.TimeQuest.Hour) % 24) || (DateTime.Now.Day < (timeStart.Day + ConfigMeneger.Configth.TimeQuest.Day + plus) % 30)) && ConfigMeneger.Configth.IsWorkingQuest == true)
            {
                if (number + 1 < PeopleList.Peoples.Count)
                {
                    for (int i = number; i < PeopleList.Peoples.Count; i++)
                    {
                        PeopleList.Peoples[i].NumberQuestions = 1;
                        PeopleList.Peoples[i].NumberQuestions = 0;

                        bot.Messages.Send(new MessagesSendParams() { UserId = PeopleList.Peoples[i].UserId, Message = QuestionsList.Questions[0].Question, RandomId = new Random().Next() });

                        number++;
                    }
                }
            }

            if (ConfigMeneger.Configth.IsWorkingQuest == true)
            {
                ConfigMeneger.Configth.IsWorkingQuest = false;
                ConfigMeneger.Configth.IsRegistretingQuest = false;
                ConfigMeneger.SaveConfigth();

                string output = Results(message, bot);

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = output, RandomId = new Random().Next() });

                return output;
            }

            return "Quest end";
        }

        private string Results(Message message, VkApi bot)
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

            Logger.Log($"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: {output}");
            return output;
        }
    }
}
