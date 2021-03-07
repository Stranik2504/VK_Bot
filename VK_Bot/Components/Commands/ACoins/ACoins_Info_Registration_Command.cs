using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK_Bot.Components.Commands.ACoins
{
    public class ACoins_Info_Registration_Command : Command
    {
        public ACoins_Info_Registration_Command() => SetNames("/acoins_registration");

        public override Visibility GetVisibility() => Visibility.Hidden;

        public override Module GetModule() => Module.ACoins;

        public override Access[] GetAccess() => SetAccess(Access.NotIndexed, Access.Programmer);

        public override string Description() => "Команда информации о регистрации";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                return "В ответ на это сообщение введите свое настоящие ФИО и ожидайте ответа".ToOutput("/registration_in_database");
            }
            catch (Exception ex) { $"[ACoins_Registration_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
