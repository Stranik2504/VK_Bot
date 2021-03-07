using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.Standard
{
    public class Change_Config_Admin_Command : Command
    {
        public Change_Config_Admin_Command() => SetNames("/conf");

        public override Visibility GetVisibility() => Visibility.Hidden;

        public override Module GetModule() => Module.Standard;

        public override Access[] GetAccess() => SetAccess(Access.Programmer);

        public override string Description() => "Команда для изменения кофигов(Для просмотра введите просто команду /conf, для изменения: /conf {Название параметра для изменения} {значение})";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (message.Split(' ').Length == 1) { return ($"Конфиг: \n IdPost: {ConfigManager.Configs.IdPost} \n KeyWord: {ConfigManager.Configs.KeyWord}\n " + 
                        $"AnswerWinTextComment: |{ConfigManager.Configs.AnswerWinTextComment}|\n AnswerLoseTextComment: |{ConfigManager.Configs.AnswerLoseTextComment}|\n " + 
                        $"AnswerLoseTextDialogue: |{ConfigManager.Configs.AnswerLoseTextDialogue}|\n ApiToken: {ConfigManager.Configs.ApiToken}\n BaseId: {ConfigManager.Configs.BaseId}\n " +
                        $"ApiTokenACoins: {ConfigManager.Configs.ApiTokenACoins}\n BaseIdACoins: {ConfigManager.Configs.BaseIdACoins}\n StartPoints: {ConfigManager.Configs.StartPoints}\n " + 
                        $"PostPoints: {ConfigManager.Configs.PostPoints}\n TextPhoto: |{ConfigManager.Configs.TextPhoto}|\n NameFont: {ConfigManager.Configs.NameFont}\n " + 
                        $"FontSize: {ConfigManager.Configs.FontSize}\n TextColor: {ConfigManager.Configs.TextColor}\n Radius: {ConfigManager.Configs.Radius}").ToOutput(); }
                else if (message.Split(' ').Length >= 3)
                {
                    var command = message.Split(' ')[1];
                    var newParams = message.Remove(0, (message.Split(' ')[0] + " " + message.Split(' ')[1] + " ").Length);

                    switch (command.ToLower())
                    {
                        case "token":
                            if (additions[Additions.UserId].ToLong() == 193292716)
                            {
                                ConfigManager.Configs.Token = newParams;
                                ConfigManager.SaveConfig();
                            }
                            break;
                        case "usertoken":
                            if (additions[Additions.UserId].ToLong() == 193292716)
                            {
                                ConfigManager.Configs.UserToken = newParams;
                                ConfigManager.SaveConfig();
                            }
                            break;
                        case "idgroup":
                            if (additions[Additions.UserId].ToLong() == 193292716)
                            {
                                ConfigManager.Configs.IdGroup = long.Parse(newParams);
                                ConfigManager.SaveConfig();
                            }
                            break;
                        case "namegroup":
                            if (additions[Additions.UserId].ToLong() == 193292716)
                            {
                                ConfigManager.Configs.NameGroup = newParams;
                                ConfigManager.SaveConfig();
                            }
                            break;
                        case "idpost":
                            ConfigManager.Configs.IdPost = long.Parse(newParams);
                            ConfigManager.SaveConfig();
                            break;
                        case "keyword":
                            ConfigManager.Configs.KeyWord = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "answerwintextcomment":
                            ConfigManager.Configs.AnswerWinTextComment = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "answerlosetextcomment":
                            ConfigManager.Configs.AnswerLoseTextComment = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "answerlosetextdialogue":
                            ConfigManager.Configs.AnswerLoseTextDialogue = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "apitoken":
                            ConfigManager.Configs.ApiToken = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "apitokenacoins":
                            ConfigManager.Configs.ApiTokenACoins = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "baseid":
                            ConfigManager.Configs.BaseId = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "baseidacoins":
                            ConfigManager.Configs.BaseIdACoins = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "startpoints":
                            ConfigManager.Configs.StartPoints = long.Parse(newParams);
                            ConfigManager.SaveConfig();
                            break;
                        case "postpoints":
                            ConfigManager.Configs.PostPoints = long.Parse(newParams);
                            ConfigManager.SaveConfig();
                            break;
                        case "textphoto":
                            ConfigManager.Configs.TextPhoto = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "namefont":
                            ConfigManager.Configs.NameFont = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "fontsize":
                            ConfigManager.Configs.FontSize = float.Parse(newParams);
                            ConfigManager.SaveConfig();
                            break;
                        case "textcolor":
                            ConfigManager.Configs.TextColor = newParams;
                            ConfigManager.SaveConfig();
                            break;
                        case "radius":
                            ConfigManager.Configs.Radius = int.Parse(newParams);
                            ConfigManager.SaveConfig();
                            break;
                        default:
                            return "Ошибка во вводе команды".ToOutput();
                    }

                    return "Параметры изменены".ToOutput();
                }

                return "Ошибка во вводе команды".ToOutput();
            }
            catch (Exception ex) { $"[Change_Config_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }
    }
}
