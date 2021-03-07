using System;
using System.Threading;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands
{
    public class Reload_Admin_Command : Command
    {
        public Reload_Admin_Command() => SetNames("/reload");

        public override Visibility GetVisibility() => Visibility.Hidden;

        public override Module GetModule() => Module.Standard;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для перезагрузки бота/конфигов(Пример: /reload {bot/conf})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length == 2)
                {
                    var command = message.Split(' ')[1].ToLower();
                    if (command == "bot")
                    {
                        ThreadPool.QueueUserWorkItem((x) => { Program.ReloadBot(); }, null);

                        return "Успешно".ToOutput();
                    }
                    else if (command == "conf")
                    {
                        ConfigManager.LoadConfig();

                        return "Успешно".ToOutput();
                    }
                }

                return "Команда введена неверно".ToOutput();
            }
            catch (Exception ex) { $"[Reload_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
