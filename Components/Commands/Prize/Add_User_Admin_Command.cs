using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Add_User_Admin_Command : Command
    {
        public Add_User_Admin_Command() => SetNames("/add_user");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для добавления юзера(Пример: /add_user {userid} {access(0 - User, 1 - Admin)} {points} {name(Если определять имя автоматически, то ничего не указывать)})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length >= 4)
                {
                    var selectUserId = long.Parse(message.Split(' ')[1]);
                    var needaccess = long.Parse(message.Split(' ')[2]);
                    var points = long.Parse(message.Split(' ')[3]);

                    Database.Log(additions[Additions.UserId].ToLong(), selectUserId, (long)points, "create");

                    bool res = false;
                    if (message.Split(' ').Length == 4)
                    {
                        res = Database.AddInDatabase((Access)needaccess, selectUserId, points);
                    }
                    else
                    {
                        var Username = message.Remove(0, (message.Split(' ')[0] + " " + message.Split(' ')[1] + " " + message.Split(' ')[2] + " " + message.Split(' ')[3] + " ").Length);
                        res = Database.AddInDatabase((Access)needaccess, Username, selectUserId, points);
                    }

                    if (res) { return "Юзер успешно добавлен".ToOutput(); }

                    return "Ошибка в добавлении юзера".ToOutput();
                }

                return "Команда введена неверно".ToOutput();
            }
            catch (Exception ex) { $"[Add_User_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
