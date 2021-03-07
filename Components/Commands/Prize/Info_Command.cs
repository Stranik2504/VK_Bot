using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Info_Command : Command
    {
        public Info_Command() => SetNames("/info");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.User, Access.Admin, Access.Programmer);

        public override string Description() => "Команда для получения информации о возможностях получении поинтов";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try { return $"При старте ты получаешь поинты({ConfigManager.Configs.StartPoints}). Ты можешь получить поинт за репост в размере {ConfigManager.Configs.PostPoints} и за каждый день после, ты будешь получать столько же. Так же ты можешь поиграть с ботом на поинты. Для игры напиши в комментариях к этому посту(https://vk.com/club{ConfigManager.Configs.IdGroup}?w=wall-{ConfigManager.Configs.IdGroup}_{ConfigManager.Configs.IdPost}) слово: {ConfigManager.Configs.KeyWord}".ToOutput(); } catch (Exception ex) { $"[Info_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
