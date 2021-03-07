using System;
using System.IO;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.ACoins
{
    public class Generate_Promocode_Admin_Command : Command
    {
        public Generate_Promocode_Admin_Command() => SetNames("/add_promocode");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.ACoins;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer, Access.Bot);

        public override string Description() => "Команда для добавления промокодов, после запроса";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (additions.ContainsKey(Additions.ReplyUserId) && additions[Additions.ReplyUserId].ToLong() == -ConfigManager.Configs.IdGroup)
                {
                    if (message.Split(' ')[4].ToLower() == "нет")
                    {
                        if (PromocodeManager.Promocodes.ContainsUserId(message.Split(' ')[1].ToLong()))
                        {
                            bool isTryOk = Bot.TrySendUser(message.Split(' ')[1].ToLong(), "Промокод не одобрен, придумайте другое слово и попробуйте ещё раз", null);

                            PromocodeManager.Promocodes.Remove(message.Split(' ')[1].ToLong());
                            PromocodeManager.SavePromocode();

                            if (!isTryOk) { $"[Generate_Promocode_Command][TrySendUser]: сообщение не отправленно".Log(); }

                            return "Промокод не одобрен".ToOutput();
                        }
                        else { return "Промокоды уже добавлены/не одобрены".ToOutput(); }
                    }
                    else if (message.Split(' ')[4].ToLower() == "да")
                    {
                        if (message.Split(' ').Length >= 7)
                        {
                            if (PromocodeManager.Promocodes.ContainsUserId(message.Split(' ')[1].ToLong()))
                            {
                                long userId = message.Split(' ')[1].ToLong();
                                string domain = message.Split(' ')[2];
                                string promocode = message.Split(' ')[3];
                                long num1 = message.Split(' ')[5].ToLong();
                                long num2 = message.Split(' ')[6].ToLong();
                                string description = message.Remove(0, (message.Split(' ')[0] + " " + message.Split(' ')[1] + " " + message.Split(' ')[2] + " " + message.Split(' ')[3] + " " + message.Split(' ')[4] + " " + message.Split(' ')[5] + " " + message.Split(' ')[6] + " ").Length);
                                string description1 = description.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                string description2 = description.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[1];

                                bool isOk = Database.CreateNewPromocode("new-" + promocode + "-event", num1, description1);
                                isOk = Database.CreateNewPromocode("new-" + promocode + "-smena", num2, description2) && isOk;

                                bool isTryAdd = Database.AddPromocodeToUser(PromocodeManager.GetById(userId).domain.Replace('\0', ' '), "new-" + promocode + "-event", "new-" + promocode + "-smena");

                                if (isTryAdd)
                                {
                                    bool isTryOk = Bot.TrySendUser(message.Split(' ')[1].ToLong(), $"Промокоды добавлены: {"new-" + promocode + "-event"}, {"new-" + promocode + "-smena"}", null);

                                    PromocodeManager.Promocodes.Remove(message.Split(' ')[1].ToLong());
                                    PromocodeManager.SavePromocode();

                                    if (!isTryOk) { $"[Generate_Promocode_Command][TrySendUser]: сообщение не отправленно".Log(); }

                                    return "Промокоды добавлены".ToOutput();
                                }
                                else { return "Ошибка, при присваивании промокода юзеру".ToOutput(); }
                            }
                            else { return "Промокоды уже добавлены/не одобрены".ToOutput(); }
                        }
                        else { return "Неправильное количество аргументов".ToOutput(); }
                    }
                }
                else
                {
                    if (PromocodeManager.Promocodes.Count > 0)
                    {
                        return ($"Юзер({"https://vk.com/" + PromocodeManager.Promocodes[0].userId}), хочет создать промокод со словом: {PromocodeManager.Promocodes[0].promocode}. Вы подтверждаете создание промокодов: {"new-" + PromocodeManager.Promocodes[0].promocode + "-event"} и {"new-" + PromocodeManager.Promocodes[0].promocode + "-smena"}(Если да, то в ответ на это сообщение отправьте \"Да {"{скидка event} {скидка smena} {Описание event}||{Описание smena}"}\". Иначе отправьте \"Нет\")").ToOutput("/add_promocode " + PromocodeManager.Promocodes[0].userId + " " + PromocodeManager.Promocodes[0].promocode);
                    }
                    else { return ("Все промокоды подтверждены").ToOutput(); }
                }
            }
            catch (Exception ex) { $"[Generate_Promocode_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
