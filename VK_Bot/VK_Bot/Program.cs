using System;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using VK_Bot.Classes;
using Classes;
using System.Threading.Tasks;

namespace VK_Bot
{
    class Program
    {
        private static Bot bot;
        private static bool isWorking;
        private static Task task;

        public static void Main(string[] args)
        {
            try
            {
                ConfigMeneger.ReLoadConfigth();
                Logger.GetParams("LogBot.log", ConfigMeneger.Configth.IsWorkingLoging);
                RegisterList.LoadListUser();
                if (ConfigMeneger.Configth.NameSave.ToLower() == "json") { AdminsList.LoadJsonListAdmins(); } else if (ConfigMeneger.Configth.NameSave.ToLower() == "xml") { AdminsList.LoadXmlListAdmins(); }
                PeopleList.LoadPeople();
                QuestionsList.LoadQuestions();

                if (ConfigMeneger.Configth.IsStartOn == true) { task = Task.Factory.StartNew(() => { if (ConfigMeneger.Configth.IsStartOn == true) { ConfigMeneger.Configth.IsStartOn = false; ConfigMeneger.SaveConfigth(); bot.Start(); } if (ConfigMeneger.Configth.IsStopOn == true) { ConfigMeneger.Configth.IsStopOn = false; ConfigMeneger.SaveConfigth(); bot.Stop(); } }); }

                bot = new Bot();

                bot.Update += SendMessageTime.Update;
                SendMessageTime.bot = bot.bot;

                isWorking = true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(start working)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(start working)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(start working)]: {ex.Message}", null);
            }

            while (isWorking == true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    var command = Console.ReadLine();
                    Console.ResetColor();
                    switch (command.ToLower())
                    {
                        case "/start":
                            bot.Start();
                            SendMessageTime.Start();
                            break;
                        case "/stop":
                            bot.Stop();
                            ConfigMeneger.Configth.IsWorkingQuest = false;
                            ConfigMeneger.SaveConfigth();
                            SendMessageTime.Stop();
                            break;
                        case "/reload_conf":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("[configth start reload]");
                            Console.ResetColor();
                            ConfigMeneger.ReLoadConfigth();
                            bot.ReLoad_Conf();
                            break;
                        case "/reload_savename":
                            if (ConfigMeneger.Configth.NameSave.ToLower() == "json") { AdminsList.LoadJsonListAdmins(); } else if (ConfigMeneger.Configth.NameSave.ToLower() == "xml") { AdminsList.LoadXmlListAdmins(); }
                            break;
                        case "/end":
                            bot.Stop();
                            ConfigMeneger.Configth.IsWorkingQuest = false;
                            isWorking = false;
                            break;
                        case "/remess":
                            ReMess();
                            break;
                        case "/info_mess":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"[message working see = {ConfigMeneger.Configth.IsWorkingSeeMessage}]");
                            Console.ResetColor();
                            Logger.Log($"[{DateTime.Now}][message working see = {ConfigMeneger.Configth.IsWorkingSeeMessage}]");
                            break;
                        case "/relog":
                            ReLog();
                            break;
                        case "/info_log":
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"[log working = {Logger.IsWorking}]");
                            Console.ResetColor();
                            Logger.Log($"[{DateTime.Now}][log working = {Logger.IsWorking}]");
                            break;
                        case "/clear_log":
                            Logger.Clear();
                            break;
                        case "/info_comm":
                            Console.WriteLine("/start, /stop, /reload_conf, /reload_savename, /end, /remess, /info_mess, /relog, /info_log, /clear_log, /wait, /id_group, /token, /m");
                            break;
                        default:
                            OtherCommand(command.ToLower());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now}][exception(while error)]: {ex.Message}");
                    Console.ResetColor();
                    Logger.Log($"[{DateTime.Now}][exception(while error)]: {ex.Message}");
                    ExceptionMove.Exception($"[{DateTime.Now}][exception(while error)]: {ex.Message}", null);
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[ending]");
            Console.ResetColor();
            Logger.Log($"[{DateTime.Now}][ending]");

            Thread.Sleep(1000);
        }

        public static void ReLog()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[log working = {(!Logger.IsWorking).ToString()}]");
                Logger.Log($"[{DateTime.Now}][log working = {(!Logger.IsWorking).ToString()}]");

                Logger.IsWorking = !Logger.IsWorking;

                ConfigMeneger.Configth.IsWorkingLoging = Logger.IsWorking;
                ConfigMeneger.SaveConfigth();

                Console.WriteLine($"[log reloaded]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][log reloaded]");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(ReLog error)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(ReLog error)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(Relog error)]: {ex.Message}", null);
            }
        }

        private static void ReMess()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[message = {(!ConfigMeneger.Configth.IsWorkingSeeMessage).ToString()}]");
                Logger.Log($"[{DateTime.Now}][log working = {(!ConfigMeneger.Configth.IsWorkingSeeMessage).ToString()}]");

                ConfigMeneger.Configth.IsWorkingSeeMessage = !ConfigMeneger.Configth.IsWorkingSeeMessage;

                ConfigMeneger.Configth.IsWorkingSeeMessage = ConfigMeneger.Configth.IsWorkingSeeMessage;
                ConfigMeneger.SaveConfigth();

                Console.WriteLine($"[message reloaded]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][message reloaded]");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(ReMess error)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(ReMess error)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(ReMess error)]: {ex.Message}", null);
            }
        }

        private static void OtherCommand(string command)
        {
            switch (command.Split(' ')[0])
            {
                case "/wait":
                    try
                    {
                        int.TryParse(command.Split(' ')[1], out int wait);
                        ConfigMeneger.Configth.Wait = wait;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"[wait = {wait.ToString()} reload]");
                        Console.ResetColor();
                        Logger.Log($"[{DateTime.Now}][wait = {wait.ToString()} reload]");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{DateTime.Now}][exception(wait new value error)]: {ex.Message}");
                        Console.ResetColor();
                        Logger.Log($"[{DateTime.Now}][exception(wait new value error)]: {ex.Message}");
                        ExceptionMove.Exception($"[{DateTime.Now}][exception(wait new value error)]: {ex.Message}", null);
                    }
                    break;
                case "/id_group":
                    try
                    {
                        ulong.TryParse(command.Split(' ')[1], out ulong idGroup);
                        ConfigMeneger.Configth.IdGroup = idGroup;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"[idGroup = {idGroup.ToString()} reload]");
                        Console.ResetColor();
                        Logger.Log($"[{DateTime.Now}][idGroup = {idGroup.ToString()} reload]");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{DateTime.Now}][exception(idGroup new value error)]: {ex.Message}");
                        Console.ResetColor();
                        Logger.Log($"[{DateTime.Now}][exception(idGroup new value error)]: {ex.Message}");
                        ExceptionMove.Exception($"[{DateTime.Now}][exception(idGroup new value error)]: {ex.Message}", null);
                    }
                    break;
                case "/token":
                    try
                    {
                        ConfigMeneger.Configth.Token = command.Split(' ')[1];

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"[Token = {command.Split(' ')[1]} reload]");
                        Console.ResetColor();
                        Logger.Log($"[{DateTime.Now}][Token = {command.Split(' ')[1]} reload]");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{DateTime.Now}][exception(Token new value error)]: {ex.Message}");
                        Console.ResetColor();
                        Logger.Log($"[{DateTime.Now}][exception(Token new value error)]: {ex.Message}");
                        ExceptionMove.Exception($"[{DateTime.Now}][exception(Token new value error)]: {ex.Message}", null);
                    }
                    break;
                case "/m":
                    try
                    {
                        long.TryParse(command.Split(' ')[1], out long id);

                        bot.SendMessage(id, command.Remove(0, (command.Split(' ')[0] + "  " + command.Split(' ')[1]).Length));
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{DateTime.Now}][exception(send message)]: {ex.Message}");
                        Console.ResetColor();
                        Logger.Log($"[{DateTime.Now}][exception(send message)]: {ex.Message}");
                        ExceptionMove.Exception($"[{DateTime.Now}][exception(send message)]: {ex.Message}", null);
                    }
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("[Incorrectly entered command]");
                    Console.ResetColor();
                    Logger.Log($"[{DateTime.Now}][Incorrectly entered command]");
                    break;
            }
        }
    }
}
