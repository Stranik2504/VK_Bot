using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace VK_Bot.Components.Commands.Prize
{
    public class Update_Prize_Admin_Command : Command
    {
        public Update_Prize_Admin_Command() => SetNames("/update_prize");

        public override Visibility GetVisibility() => Visibility.Visible;

        public override Module GetModule() => Module.Prize;

        public override Access[] GetAccess() => SetAccess(Access.Admin, Access.Programmer);

        public override string Description() => "Команда для обновления параметров приза(Пример: /update_prize {Id приза} {Текст, при получении приза(если надо обновить)})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                bool isUpdated = false;

                if (message.Split(' ').Length > 1)
                {
                    Database.SetValueData(Place.Prizes, message.Split(' ')[1], "Text", message.Remove(0, (message.Split(' ')[0] + message.Split(' ')[1] + "  ").Length));
                    isUpdated = true;
                }

                if (additions[Additions.Attachments] != "")
                {
                    var name = Database.GetValueData<string>(Place.Prizes, message.Split(' ')[1], nameSearchField: "Photo").Field;

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFileAsync(new Uri(additions[Additions.Attachments]), name);
                    }

                    isUpdated = true;
                }

                if (message.Split(' ').Length == 1 && !isUpdated)
                {
                    string output = "";
                    int number = 1;

                    Database.GetCells(Place.Prizes).Select((x) => { output += $"{number}) Id: {x.Fields["Id"]} - Text: {x.Fields["Text"]}\n"; number++; return true; });

                    return ("Призы:\n" + output).ToOutput();
                }

                if (isUpdated == true) { return ("Параметр(-ы) приза обновлён").ToOutput(); }
                
                return "Ошибка во вводе команды".ToOutput();
            }
            catch (Exception ex) { $"[Update_Prize_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
