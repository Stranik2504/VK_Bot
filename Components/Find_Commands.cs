using System;
using System.Collections.Generic;
using System.Linq;
using VkNet.Model.Attachments;
using System.Text;
using System.Threading.Tasks;
using VK_Bot.Components.Commands;
using VkNet;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;

namespace VK_Bot.Components
{
    public static class Find_Commands
    {
        public static Output Execute(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                string PossibleNameCommand = "";
                string NameCommand = message.Split(' ')[0];
                var access = additions.ContainsKey(Additions.ReplyUserId) && additions[Additions.ReplyUserId].ToLong() == -ConfigManager.Configs.IdGroup ? Access.Bot : Database.GetAccessInDatabase(additions[Additions.UserId]);

                foreach (var command in GetCommand.GetCommands(access))
                {
                    if (command.GetNames().Contains(NameCommand))
                    {
                        return command.Move(message, additions);
                    }
                    else if ((LevenshteinDistance(command.GetNames().First(), NameCommand) * 100 / (float)command.GetNames().First().Length) <= 20)
                    {
                        PossibleNameCommand = command.GetNames().First();
                    }
                }

                if (access == Access.NotIndexed) { return "Для использования команд надо зарегистрироваться)".ToOutput(); }

                if (PossibleNameCommand != "") { return ("Команды не существует. Возможно вы имели в виду: " + PossibleNameCommand).ToOutput(); }

                return "Команды не существует".ToOutput();
            }
            catch (Exception ex) { $"[Find_Commands][Execute]: {ex.Message}".Log(); return "Ошибка".ToOutput(); }
        }

        private static int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;

        private static int LevenshteinDistance(string firstWord, string secondWord)
        {
            try
            {
                var n = firstWord.Length + 1;
                var m = secondWord.Length + 1;
                var matrixD = new int[n, m];

                const int deletionCost = 1;
                const int insertionCost = 1;

                for (var i = 0; i < n; i++)
                {
                    matrixD[i, 0] = i;
                }

                for (var j = 0; j < m; j++)
                {
                    matrixD[0, j] = j;
                }

                for (var i = 1; i < n; i++)
                {
                    for (var j = 1; j < m; j++)
                    {
                        var substitutionCost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                        matrixD[i, j] = Minimum(matrixD[i - 1, j] + deletionCost,          // удаление
                                                matrixD[i, j - 1] + insertionCost,         // вставка
                                                matrixD[i - 1, j - 1] + substitutionCost); // замена
                    }
                }

                return matrixD[n - 1, m - 1];
            }
            catch (Exception ex) { $"[Command][LevenshteinDistance]: {ex.Message}".Log(); return 0; }
        }
    }
}
