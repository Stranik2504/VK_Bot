using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Points_Command : Command
    {
        public Points_Command() => SetNames("/points");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.User);

        public override string Description() => "Команда для получения поинтов";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                return ("У вас сейчас поинтов на счету: " + Database.GetValueData<long>(Place.Users, additions[Additions.UserId], nameSearchField: "Points").Field.ToString()).ToOutput();
            }
            catch (Exception ex) { $"[Points_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
