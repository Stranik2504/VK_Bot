using System;
using System.IO;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.ACoins
{
    public class Generate_Promocode_Command : Command
    {
        public Generate_Promocode_Command() => SetNames("/generate_promocode");

        public override Visibility GetVisibility() => Visibility.Hidden;

        public override Module GetModule() => Module.ACoins;

        public override Access[] GetAccess() => SetAccess(Access.NotIndexed, Access.Admin, Access.Programmer, Access.Bot);

        public override string Description() => "Команда для генерации промокода";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (!PromocodeManager.Promocodes.ContainsUserId(additions[Additions.UserId].ToLong()))
                {
                    if (new Promocode_Command().GetPromocode(additions[Additions.Domain]) == "")
                    {
                        if (message.Split(' ').Length == 2)
                        {
                            string name_promocode = message.Split(' ')[1];

                            if (Database.GetValueData<string>(Place.Promocode, "new-" + name_promocode + "-event").Id == null && Database.GetValueData<string>(Place.Promocode, "new-" + name_promocode + "-smena").Id == null)
                            {
                                bool isTryOk = Bot.TrySendTriggerAdmins($"Юзер({"https://vk.com/" + additions[Additions.Domain]}), хочет создать промокод со словом: {name_promocode}. Вы подтверждаете создание промокодов: {"new-" + name_promocode + "-event"} и {"new-" + name_promocode + "-smena"}(Если да, то в ответ на это сообщение отправьте \"Да {"{скидка event} {скидка smena} {Описание event}||{Описание smena}"}\". Иначе отправьте \"Нет\")", "/add_promocode " + additions[Additions.UserId] + " " + additions[Additions.Domain].Replace(' ', '\0') + " " + name_promocode);

                                PromocodeManager.Promocodes.Add((additions[Additions.UserId].ToLong(), additions[Additions.Domain], name_promocode));
                                PromocodeManager.SavePromocode();

                                if (!isTryOk) { $"[Generate_Promocode_Command][TrySendTriggerAdmins]: сообщение не отправленно".Log(); }

                                return "Заявка отправлена. Ожидайте подтверждение администрации в течение двух дней".ToOutput();
                            }
                            else { return "Такой промокод уже создан, придумайте новое слово".ToOutput(); }
                        }
                        else { return "Неправильный ввод".ToOutput(); }
                    }
                    else { return "Ваш промокод уже создан".ToOutput(); }
                }
                else { return "Запрос уже отправлен".ToOutput(); }
            } 
            catch (Exception ex) { $"[Generate_Promocode_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
