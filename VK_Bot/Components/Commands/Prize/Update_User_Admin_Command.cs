using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Update_User_Admin_Command : Command
    {
        public Update_User_Admin_Command() => SetNames("/update_user");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для обновления данных юзера(Пример: /update_user {userid} {параметр(userid/access/username/reposted(это firstreposted(bool)))} {на что заменить})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length == 4)
                {
                    var selectUserId = long.Parse(message.Split(' ')[1]);
                    var newParams = message.Remove(0, (message.Split(' ')[0] + " " + message.Split(' ')[1] + " " + message.Split(' ')[2] + " ").Length);

                    switch (message.Split(' ')[2].ToLower())
                    {
                        case "userid":
                            if (Database.SetValueData(Place.Users, selectUserId.ToString(), "UserId", long.Parse(newParams))) { return "Успешно".ToOutput(); }
                            break;
                        case "access":
                            if (Database.SetValueData(Place.Users, selectUserId.ToString(), "Access", int.Parse(newParams))) { return "Успешно".ToOutput(); }
                            break;
                        case "username":
                            if (Database.SetValueData(Place.Users, selectUserId.ToString(), "Username", newParams)) { return "Успешно".ToOutput(); }
                            break;
                        case "repost":
                        case "reposted":
                        case "firstreposted":
                            if (Database.SetValueData(Place.Users, selectUserId.ToString(), "FirstReposted", bool.Parse(newParams))) { return "Успешно".ToOutput(); }
                            break;
                        default:
                            return "Ошибка во вводе команды".ToOutput();
                    }
                }

                return "Ошибка во вводе команды".ToOutput();
            }
            catch (Exception ex) { $"[Update_User_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
