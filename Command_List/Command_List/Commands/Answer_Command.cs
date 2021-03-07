using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;

namespace Command_List.Commands
{
    public class Answer_Command : Command
    {
        public override string[] NameCommand => List("/answer", "/ответ");

        public override string NameClass => "Ответить";

        public override string Explanation => "/answer {Ответ на вопрос}";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            if (ConfigMeneger.Configth.IsWorkingQuest == true)
            {
                if (message.Text.Split(' ').Length > 1)
                {
                    int numberPeople = 0;

                    foreach (var people in PeopleList.Peoples)
                    {
                        if (people.UserId == message.PeerId.Value)
                        {
                            break;
                        }

                        numberPeople++;
                    }

                    bool correct = false;

                    foreach (var answer in QuestionsList.Questions[PeopleList.Peoples[numberPeople].NumberQuestions - 1].Answers)
                    {
                        if (message.Text.Remove(0, (message.Text.Split(' ')[0] + " ").Length).ToLower().StartsWith(answer.ToLower()) == true && message.Text.Remove(0, (message.Text.Split(' ')[0] + " ").Length).ToLower().Contains(answer.ToLower()))
                        {
                            correct = true;
                            break;
                        }
                    }

                    if (correct == true)
                    {
                        if (PeopleList.Peoples[numberPeople].NumberQuestions < QuestionsList.Questions.Count)
                        {
                            PeopleList.Peoples[numberPeople].CorrectAnswer++;
                            PeopleList.Peoples[numberPeople].NumberQuestions++;
                            PeopleList.SavePeople();

                            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Вопрос: {QuestionsList.Questions[PeopleList.Peoples[numberPeople].NumberQuestions - 1].Question}", RandomId = new Random().Next() });

                            return $"Question: {QuestionsList.Questions[PeopleList.Peoples[numberPeople].NumberQuestions - 1].Question}";
                        }
                        else
                        {
                            PeopleList.Peoples[numberPeople].CorrectAnswer++;
                            PeopleList.Peoples[numberPeople].TimeEnd = DateTime.Now;
                            PeopleList.SavePeople();

                            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = $"Ты прошёл квест", RandomId = new Random().Next() });

                            return "You ended quest";
                        }
                    }
                    else
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Неправильный ответ", RandomId = new Random().Next() });

                        return "Not correct answer";
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
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Квест не работает", RandomId = new Random().Next() });

                return "Quest not work";
            }
        }
    }
}
