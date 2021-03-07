using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Delete_User_Admin_Command : Command
    {
        public Delete_User_Admin_Command() => SetNames("/delete_user");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для удаления юзера";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length == 2)
                {
                    if (Database.DeleteCellInDatabase(Place.Users, message.Split(' ')[1])) { return "Успешно".ToOutput(); } else { return "Ошибка при удалении юзера".ToOutput(); }
                }

                return "Ошибка во вводе команды".ToOutput();
            }
            catch (Exception ex) { $"[Delete_User_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
