using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace VK_Bot.Components.Commands.ACoins
{
    public class ACoins_Command : Command
    {
        public ACoins_Command() => SetNames("/acoins");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.ACoins;

        public override Access[] GetAccess() => SetAccess(Access.User, Access.Admin, Access.Programmer);

        public override string Description() => "Команда для получения количества ACoins";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try { return ("У вас сейчас на счету " + Database.GetValueData<long>(Place.Wallet, Database.GetValueData<JArray>(Place.ClubCard, additions[Additions.Domain], nameSearchField: "Кошелек").Field.First().ToString(), nameSearchField: "Поинты Rollup (from Операции)").Field.ToString().ToString() + " ACoins").ToOutput(); } catch (Exception ex) { $"[ACoins_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
