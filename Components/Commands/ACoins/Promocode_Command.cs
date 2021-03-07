using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.ACoins
{
    public class Promocode_Command : Command
    {
        public Promocode_Command() => SetNames("/promocode");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.ACoins;

        public override Access[] GetAccess() => SetAccess(Access.User, Access.Admin, Access.Programmer);

        public override string Description() => "Команда для получения промокода";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try { var promocode = GetPromocode(additions[Additions.Domain]); if (promocode == "" || promocode == " " || promocode == null) { return "Для создания промокода в ответ на это сообщение введите !!одно!! слово на латинском и ожидайте подтверждения модерации".ToOutput("/generate_promocode"); } return ("Ваш промокод(-ы): " + promocode).ToOutput(); } catch (Exception ex) { $"[Promocode_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }

        public string GetPromocode(string domain)
        {
            try
            {
                string output = "";
                var promocodes = Database.GetValueData<JArray>(Place.ClubCard, domain, nameSearchField: "Промокоды");

                if (promocodes.Field != null)
                {
                    foreach (var promocode_id in promocodes.Field)
                    {
                        output += Database.GetValueData<string>(Place.Promocode, promocode_id.ToString()).Field + ", ";
                    }

                    if (output.Length > 0) { output = output.Remove(output.Length - 3, 2); }
                }

                return output;
            }
            catch (Exception ex) { $"[Promocode_Command][GetPromocode]: {ex.Message}".Log(); }

            return "";
        }
    }
}
