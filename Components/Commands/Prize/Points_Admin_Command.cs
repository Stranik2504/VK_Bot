using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Points_Admin_Command : Command
    {
        public Points_Admin_Command() => SetNames("/points");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для получения поинтов, получения всех поинтов юзеров(all), получения юзера с максимальным числом поинтов(max), получения числа поинтов у юзера(get {userId}), обнуления числа поинтов(zeroize {userId}), для добавление/установки/уменьшения(add/set/reduce {userId} {points})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length == 1) { return new Points_Command().Move(message, additions); }
                else
                {
                    int number = 1;
                    string output = "";

                    switch (message.Split(' ')[1])
                    {
                        case "all":
                            foreach (var item in Database.GetCells(Place.Users)) { output += $"{number}) Name: {item.Fields["Username"]}(UserId: {item.Fields["UserId"]}) - Points: {item.Fields["Points"]}\n"; number++; }
                            return output.ToOutput();
                        case "max":
                            long max = 0;

                            foreach (var item in Database.GetCells(Place.Users))
                            {
                                if (long.Parse(item.Fields["Points"].ToString()) > max)
                                {
                                    number = 1;
                                    max = long.Parse(item.Fields["Points"].ToString());
                                    output = $"{number}) Name: {item.Fields["Username"]}(UserId: {item.Fields["UserId"]}) - Points: {item.Fields["Points"]}\n";
                                }
                                else if (long.Parse(item.Fields["Points"].ToString()) == max)
                                {
                                    number++;
                                    output += $"{number}) Name: {item.Fields["Username"]}(UserId: {item.Fields["UserId"]}) - Points: {item.Fields["Points"]}\n";
                                }
                            }

                            return output.ToOutput();
                        case "get":
                            if (message.Split(' ').Length == 3)
                            {
                                var seletUserId = long.Parse(message.Split(' ')[2]);
                                string name = Database.GetValueData<string>(Place.Users, seletUserId.ToString(), nameSearchField: "Username").Field;
                                string points = Database.GetValueData<long>(Place.Users, seletUserId.ToString(), nameSearchField: "Points").Field.ToString();

                                return $"Name: {name}(UserId: {seletUserId}) - Points: {points}".ToOutput();
                            }
                            break;
                        case "zeroize":
                            if (message.Split(' ').Length == 3)
                            {
                                var selectUserId = long.Parse(message.Split(' ')[2]);

                                Database.Log(additions[Additions.UserId].ToLong(), selectUserId, 0, "zeroize");
                                if (Database.SetValueData(Place.Users, selectUserId.ToString(), "Points", 0)) { return "Успешно".ToOutput(); }

                                return "Ошибка в обнулении поинтов".ToOutput();
                            }
                            break;
                        case "add":
                            if (message.Split(' ').Length == 4)
                            {
                                var selectUserId = long.Parse(message.Split(' ')[2]);
                                var points = long.Parse(message.Split(' ')[3]);

                                Database.Log(additions[Additions.UserId].ToLong(), selectUserId, points, "add");
                                if (Database.SetValueData(Place.Users, selectUserId.ToString(), "Points", Database.GetValueData<long>(Place.Users, selectUserId.ToString(), nameSearchField: "Points").Field + points)) { return "Успешно".ToOutput(); }

                                return "Ошикба в обнулении поинтов".ToOutput();
                            }
                            break;
                        case "reduce":
                            if (message.Split(' ').Length == 4)
                            {
                                var selectUserId = long.Parse(message.Split(' ')[2]);
                                var points = long.Parse(message.Split(' ')[3]);
                                var userPoints = Database.GetValueData<long>(Place.Users, selectUserId.ToString(), nameSearchField: "Points").Field;
                                if (userPoints - points < 0) { points = 0; } else { points = userPoints - points; }

                                Database.Log(additions[Additions.UserId].ToLong(), selectUserId, points, "reduce");
                                if (Database.SetValueData(Place.Users, selectUserId.ToString(), "Points", points)) { return "Успешно".ToOutput(); }

                                return "Ошибка в обнулении поинтов".ToOutput();
                            }
                            break;
                        case "set":
                            if (message.Split(' ').Length == 4)
                            {
                                var selectUserId = long.Parse(message.Split(' ')[2]);
                                var points = long.Parse(message.Split(' ')[3]);
                                if (points < 0) { points = 0; }

                                Database.Log(additions[Additions.UserId].ToLong(), selectUserId, points, "set");
                                if (Database.SetValueData(Place.Users, selectUserId.ToString(), "Points", points)) { return "Успешно".ToOutput(); }

                                return "Ошибка в обнулении поинтов".ToOutput();
                            }
                            break;
                    }

                    return "Команда введена неверно".ToOutput();
                }
            }
            catch (Exception ex) { $"[Points_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
