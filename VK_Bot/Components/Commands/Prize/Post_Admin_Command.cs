using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Prize
{
    public class Post_Admin_Command : Command
    {
        public Post_Admin_Command() => SetNames("/post");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для изменения отслеживаемого поста(Пример: /post {id post})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length == 2)
                {
                    var newParams = long.Parse(message.Split(' ')[1]);

                    ConfigManager.Configs.IdPost = newParams;
                    ConfigManager.SaveConfig();

                    return "Успешно".ToOutput();
                }

                return "Ошибка во вводе команды".ToOutput();
            }
            catch (Exception ex) { $"[Post_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
