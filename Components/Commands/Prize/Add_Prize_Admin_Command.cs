using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace VK_Bot.Components.Commands.Prize
{
    public class Add_Prize_Admin_Command : Command
    {
        public Add_Prize_Admin_Command() => SetNames("/add_prize");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для добавление приза";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (additions[Additions.Attachments] != "")
                {
                    long max = 1;

                    foreach (var item in Directory.GetFiles("Photo"))
                    {
                        var num = long.Parse(item.Split('\\')[item.Split('\\').Length - 1].Split('.')[0].ToString());
                        if (num > max) { max = num; }
                    }

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri(additions[Additions.Attachments]), $@"Photo\{max + 1}.jpg");
                    }

                    if (Database.AddPrizeInDatabase(message.Remove(0, (message.Split(' ')[0] + " ").Length), $@"{max + 1}.jpg")) { return "Приз успешно добавлен".ToOutput(); } else { return "Ошибка при добавлении приза".ToOutput(); }
                }

                return "Нет фотографии для приза".ToOutput();
            }
            catch (Exception ex) { $"[Add_Prize_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
