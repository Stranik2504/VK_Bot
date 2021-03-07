using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Standard
{
    public class GetCommands_Commad : Command
    {
        public GetCommands_Commad() => SetNames("/commands", "/help");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Standard;

        public override Access[] GetAccess() => SetAccess(Access.User, Access.Admin, Access.Programmer, Access.Bot);

        public override string Description() => "Команда для вывода всех команд";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                string output = "";
                int number = 1;

                var access = Database.GetAccessInDatabase(additions[Additions.UserId]);

                foreach (var command in GetCommand.GetCommands(access))
                {
                    if (output.Length < 3000 && (command.GetVisibility() == Visibility.Visible || access == Access.Programmer))
                    {
                        output += $"{number}) ";
                        for (int i = 0; i < command.GetNames().Length; i++) { output += command.GetNames()[i]; if (i + 1 < command.GetNames().Length) { output += ", "; } }
                        output += " - ";
                        output += $"{command.Description()}\n";
                        number++;
                    }
                }

                return ("Команды:\n" + output).ToOutput();
            }
            catch (Exception ex) { $"[GetCommands_Commad]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
