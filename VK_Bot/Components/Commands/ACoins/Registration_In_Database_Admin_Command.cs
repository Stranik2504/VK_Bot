using System;
using System.Collections.Generic;

using static System.Diagnostics.Debug;

namespace VK_Bot.Components.Commands.ACoins
{
    public class Registration_In_Database_Admin_Command : Command
    {
        public Registration_In_Database_Admin_Command() => SetNames("/add_user");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.ACoins;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer, Access.Bot);

        public override string Description() => "Команда для добавление юзеров, которые хотят зарегистрироваться";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (additions.ContainsKey(Additions.ReplyUserId) && additions[Additions.ReplyUserId].ToLong() == -192454284)
                {
                    long userId = message.Split(' ')[1].ToLong();
                    string domain = message.Split(' ')[2];
                    string fio = message.Split(new string[] { "{{" }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new string[] { "}}" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    string input = message.Remove(0, (message.Split(' ')[0] + " " + message.Split(' ')[1] + " " + domain + " {{" + fio + "}} ").Length);

                    if (input.Split(' ')[0].ToLower() == "нет")
                    {
                        if (RegistrationManager.Users.ContainsUserId(userId))
                        {
                            bool isTryOk = Bot.TrySendUser(userId, "Ваша заявка не одобрена", null);

                            RegistrationManager.Users.Remove(userId);
                            RegistrationManager.SaveUsers();

                            if (!isTryOk) { $"[Generate_Promocode_Command][TrySendUser]: сообщение не отправленно".Log(); }

                            return "Юзер не добавлен".ToOutput();
                        }
                        else { return "Юзер уже добавлен/не добавлен".ToOutput(); }
                    }
                    else if (input.Split(' ')[0].ToLower() == "да")
                    {
                        if (input.Split(' ').Length >= 1)
                        {
                            if (RegistrationManager.Users.ContainsUserId(userId))
                            {
                                bool isTryAdd = false;

                                if (input.Split(' ').Length == 1)
                                {
                                    isTryAdd = Database.AddUser(fio, domain);
                                }
                                else if (input.Length >= 3)
                                {
                                    if (input.Split(' ')[1].ToLower() == "card")
                                    {
                                        string numberCard = input.Split(' ')[2].ToLower();
                                        isTryAdd = Database.AddLinkToCard(numberCard, domain);
                                    }
                                    else
                                    {
                                        string newFio = input.Remove(0, (input.Split(' ')[0] + " ").Length);
                                        isTryAdd = Database.AddClubCard(newFio, domain);
                                    }
                                }
                                else
                                {
                                    return "Неправильное количество аргументов".ToOutput();
                                }

                                if (isTryAdd)
                                {
                                    bool isTryOk = Bot.TrySendUser(userId, $"Регистрация подтверждена", null);

                                    RegistrationManager.Users.Remove(userId);
                                    RegistrationManager.SaveUsers();

                                    if (!isTryOk) { $"[Registration_In_Database_Admin_Command][TrySendUser]: сообщение не отправленно".Log(); }

                                    return "Юзер добавлен".ToOutput();
                                }
                                else { return "Ошибка, при создании/изменении полей".ToOutput(); }
                            }
                            else { return "Юзер уже добавлен/не добавлен".ToOutput(); }
                        }
                        else { return "Неправильное количество аргументов".ToOutput(); }
                    }
                }
                else
                {
                    if (RegistrationManager.Users.Count > 0)
                    {
                        return new Registration_In_Database_Command().AdminMessage(RegistrationManager.Users[0].userId.ToString(), RegistrationManager.Users[0].domain, RegistrationManager.Users[0].name);
                    }
                    else { return "Все юзеры добавлены".ToOutput(); }
                }
            }
            catch (Exception ex) { $"[Registration_In_Database_Admin_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
