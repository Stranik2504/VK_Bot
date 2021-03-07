using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VK_Bot.Components;

using static System.Diagnostics.Debug;

namespace VK_Bot
{
    class Program
    {
        public const string ErrorLog = @"files/Errors.log";

        public static event Action reloadBot = () => { _bot.StopBot(); _bot.StartBot(); };

        private readonly static Bot _bot = new Bot();

        static void Main(string[] args)
        {
            try
            {
                LoadParams();

                BotWork();

                Console.ReadKey();
            }
            catch (Exception ex) {$"[Main]: {ex.Message}".Log(); }
        }

        public static void ReloadBot() { try { reloadBot(); } catch { } }

        private static void Commands()
        {
            string text = "";
            while (text != "/exit")
            {
                text = Console.ReadLine();
                string output = "";

                switch (text.Split(' ')[0])
                {
                    case "/reload":
                        ConfigManager.LoadConfig();
                        PromocodeManager.LoadPromocode();
                        _bot.StopBot();
                        _bot.StartBot();
                        output = "All reloaded";
                        break;
                    case "/reload_conf":
                        ConfigManager.LoadConfig();
                        output = "Config reloaded";
                        break;
                    case "/reload_bot":
                        _bot.StopBot();
                        _bot.StartBot();
                        output = "Bot reloaded";
                        break;
                    case "/reload_promocode":
                        PromocodeManager.LoadPromocode();
                        output = "Promocodes reloaded";
                        break;
                    case "/end":
                        _bot.StopBot();
                        output = "Bot stoped";
                        break;
                    case "/clear_log":
                        StreamWriter writerLogs = new StreamWriter(ErrorLog);
                        writerLogs.Close();
                        output = "Logs cleared";
                        break;
                    case "/info_errors":
                        StreamReader readerLogs = new StreamReader(ErrorLog);
                        Console.WriteLine(readerLogs.ReadToEnd());
                        break;
                    case "/clear_data":
                        StreamWriter writerData = new StreamWriter(Database.DataName);
                        writerData.Close();
                        output = "Data long cleared";
                        break;
                    case "/clear":
                        StreamWriter writerall = new StreamWriter(ErrorLog);
                        writerall.Close();
                        writerall = new StreamWriter(Database.DataName);
                        writerall.Close();
                        output = "All logs cleared";
                        break;
                    case "/add_tester":
                        var testerIdAdd = long.Parse(text.Split(' ')[1]);
                        ConfigManager.Configs.TestersIds.Add(testerIdAdd);
                        ConfigManager.SaveConfig();

                        output = "Tester added";
                        break;
                    case "/remove_tester":
                        var testerIdRemove = long.Parse(text.Split(' ')[1]);
                        if (ConfigManager.Configs.TestersIds.Contains(testerIdRemove)) { ConfigManager.Configs.TestersIds.Remove(testerIdRemove); ConfigManager.SaveConfig(); output = "Tester removed"; }
                        else { output = "Tester not found"; }
                        break;
                    default:
                        output = "The command is entered incorrectly!";
                        break;
                }

                if (text != "/exit")
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(output);
                    Console.ResetColor();
                }
            }
        }

        private static void BotWork()
        {
            _bot.StartBot();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Bot started");
            Console.ResetColor();

            Commands();

            _bot.StopBot();
            Database.End();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Exit...");
            Console.ResetColor();
        }

        private static void LoadParams()
        {
            ConfigManager.LoadConfigAction += _bot.ReloadParams;
            ConfigManager.LoadConfigAction += () => { if (ConfigManager.Configs.IsActivateRaffle) { if (Directory.Exists("Avatar")) { Directory.CreateDirectory("Avatar"); } if (Directory.Exists("Photo")) { Directory.CreateDirectory("Photo"); } } };
            ConfigManager.LoadConfig();

            PromocodeManager.LoadPromocode();
            RegistrationManager.LoadUsers();

            Database.Start(ConfigManager.Configs.IsLoadRaffleClient, ConfigManager.Configs.IsActivateACoins);
        }
    }

    public static class Addition
    {
        public static void Log(this string log)
        {
            try { File.AppendAllText(Program.ErrorLog, "[" + DateTime.Now.ToString() + "]" + log + " \n"); } catch { }
        }

        public static long ToLong(this string num)
        {
            bool res = long.TryParse(num, out long number);
            return res ? number : -1;
        }

        public static bool IsLetters(this string num)
        {
            for (int i = 0; i < num.Length; i++)
                if (!char.IsLetter(num[i]) && num[i] != ' ')
                    return false;           
            
            return true;
        }

        public static void AddCommands(this List<Command> commands, IReadOnlyList<Command> newCommands)
        {
            foreach (var new_command in newCommands)
            {
                List<string> nameCommands = new List<string>();
                nameCommands.AddRange(new_command.GetNames());

                foreach (var command in commands)
                {
                    command.GetNames().RemoveIntersacts(new_command);
                }

                if (nameCommands.Count > 0) { commands.Add(new_command); }
            }
        }

        private static void RemoveIntersacts(this string[] num, Command command)
        {
            for (int i = 0; i < command.GetNames().Length; i++)
            {
                if (num.Contains(command.GetNames()[i])) { command.RemoveNameAt(i); i--; }
            }
        }

        public static Output ToOutput(this string answer) => new Output(answer);

        public static Output ToOutput(this string answer, string payload) => new OPayload(payload, answer);

        public static bool ContainsUserId(this List<(long userId, string domain, string promocode)> list, long userId)
        {
            foreach (var item in list) { if (item.userId == userId) { return true; } }

            return false;
        }

        public static bool Contains(this Access[] accesses, Access access)
        {
            foreach (var item in accesses) { if (item == access) { return true; } }

            return false;
        }

        public static bool Remove(this List<(long userId, string domain, string promocode)> list, long userId)
        {
            for (int i = 0; i < list.Count; i++) { if (list[i].userId == userId) { list.RemoveAt(i); return true; } }

            return false;
        }

        public static string ToAirtableDate(this string num)
        {
            string[] nums = num.Split('.');

            if (nums[0].Length == 2 && nums[0][0] == '0') { try { nums[0] = nums[0].Remove(0, 1); } catch { } }
            if (nums[1].Length == 2 && nums[1][0] == '0') { try { nums[1] = nums[1].Remove(0, 1); } catch { } }

            return nums[1] + "." + nums[0] + "." + nums[2];  
        }

        public static Dictionary<string, object> ToDictionary<T>(this (string NameField, T Param) field)
        {
            Dictionary<string, object> newFields = new Dictionary<string, object>();

            newFields.Add(field.NameField, field.Param);

            return newFields;
        }

        public static string ToNumber(this DateTime dateTime)
        {
            return dateTime.Day.IntToNormalTime() + dateTime.Month.IntToNormalTime() + dateTime.Year.ToString() + "-" + dateTime.Hour.IntToNormalTime() + dateTime.Minute.IntToNormalTime() + dateTime.Second.IntToNormalTime();
        }

        public static string IntToNormalTime(this int time)
        {
            if (time < 10) { return "0" + time.ToString(); } else { return time.ToString(); }
        }
    }
}
