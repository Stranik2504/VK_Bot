using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Roll_Command : Command
    {
        public Roll_Command() => SetNames("/roll");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.User, Access.Admin, Access.Programmer);

        public override string Description() => "Команда для розыгрыша поинтов(Прописываться: /roll {количество поинтов(0 - розыгрыш всех поинтов)})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length == 2)
                {
                    if (long.TryParse(message.Split(' ')[1], out long points))
                    {
                        if (points <= Database.GetValueData<long>(Place.Users, additions[Additions.UserId], nameSearchField: "Points").Field)
                        {
                            var nowPoints = Database.GetValueData<long>(Place.Users, additions[Additions.UserId], nameSearchField: "Points").Field;
                            bool IsWin = RandChance();

                            if (points > 0)
                            {
                                if (IsWin == true)
                                {
                                    Database.Log(-1, additions[Additions.UserId].ToLong(), nowPoints + points * 2, $"roll win {points}");
                                    Database.SetValueData(Place.Users, additions[Additions.UserId], "Points", nowPoints + points);

                                    return $"Поздравляю, ты выиграл(+{points * 2})".ToOutput();
                                }
                                else
                                {
                                    Database.Log(-1, additions[Additions.UserId].ToLong(), nowPoints - points, $"roll lose {points}");
                                    Database.SetValueData(Place.Users, additions[Additions.UserId], "Points", nowPoints - points);

                                    return $"К сожалению ты проиграл(-{points})".ToOutput();
                                }
                            }
                            else if (points == 0)
                            {
                                if (IsWin == true)
                                {
                                    Database.Log(-1, additions[Additions.UserId].ToLong(), nowPoints + points * 2, $"roll win {nowPoints * 2}");
                                    Database.SetValueData(Place.Users, additions[Additions.UserId], "Points", nowPoints * 2);

                                    return $"Поздравляю, ты удвоил свои поинты({Database.GetValueData<long>(Place.Users, additions[Additions.UserId], "Points").ToString()})".ToOutput();
                                }
                                else
                                {
                                    Database.Log(-1, additions[Additions.UserId].ToLong(), 0, $"roll lose {nowPoints}");
                                    Database.SetValueData(Place.Users, additions[Additions.UserId], "Points", 0);

                                    return $"К сожалению ты проиграл все поинты".ToOutput();
                                }
                            }

                            return "Ошибка во вводе команды".ToOutput();
                        }

                        return "У вас недостаточно поинтов".ToOutput();
                    }
                }

                return "Ошибка во вводе команды".ToOutput();
            }
            catch (Exception ex) { $"[Roll_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }

        private bool RandChance() => new Random().Next(1, 101) <= 50 ? true : false;
    }
}
