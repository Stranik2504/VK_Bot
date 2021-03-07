using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Delete_Prize_Admin_Command : Command
    {
        public Delete_Prize_Admin_Command() => SetNames("/update_prize");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для удаления приза(Пример: /update_prize {Id приза})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length == 1)
                {
                    string output = "";
                    int number = 1;

                    foreach (var item in Database.GetCells(Place.Prizes)) { output += $"{number}) Id: {item.Fields["Id"]} - Text: {item.Fields["Text"]}\n"; number++; }

                    return ("Призы:\n" + output).ToOutput();
                }
                else { if (Database.DeleteCellInDatabase(Place.Prizes, message.Split(' ')[1])) { return "Приз удален".ToOutput(); } return "Ошибка при удалении приза".ToOutput(); }
            }
            catch (Exception ex) { $"[Delete_Prize_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
