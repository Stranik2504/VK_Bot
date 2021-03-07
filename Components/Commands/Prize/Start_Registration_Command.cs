using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Start_Registration_Command : Command
    {
        public Start_Registration_Command() => SetNames("/start", "/начать", "начать", "start");

        public override Visibility GetVisibility() => Visibility.Hidden;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.NotIndexed, Access.User, Access.Admin, Access.Programmer);

        public override string Description() => "Команда для регистрации";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (Database.IsInDatabase(Place.Users, additions[Additions.UserId]) == false)
                {
                    if (Database.AddInDatabase(Access.User, additions[Additions.UserId].ToLong()))
                    {
                        Database.Log(-1, additions[Additions.UserId].ToLong(), ConfigManager.Configs.StartPoints, "start add");

                        return $"Вы успешно зарегистрированы, для игры напиши в комментариях к этому посту(https://vk.com/club{ConfigManager.Configs.IdGroup}?w=wall-{ConfigManager.Configs.IdGroup}_{ConfigManager.Configs.IdPost}) слово: {ConfigManager.Configs.KeyWord}".ToOutput();
                    }
                    else { return "Ошибка регистрации".ToOutput(); }
                }
                else
                {
                    return $"Вы уже зарегистрированы, для игры напиши в комментариях к этому посту(https://vk.com/club{ConfigManager.Configs.IdGroup}?w=wall-{ConfigManager.Configs.IdGroup}_{ConfigManager.Configs.IdPost}) слово: {ConfigManager.Configs.KeyWord}".ToOutput();
                }
            }
            catch (Exception ex) { $"[Start_Registration_Command]: {ex.Message}".Log(); }

            return "Oшибка".ToOutput();
        }
    }
}
