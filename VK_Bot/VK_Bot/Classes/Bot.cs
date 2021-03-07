using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Threading;
using Command_List;

namespace VK_Bot.Classes
{
    public class Bot
    {
        public bool IsActive { get; private set; }

        public VkApi bot { get; private set; }

        public event Action<VkApi> Update;

        private LongPollServerResponse longPoll;

        private Task task;


        private IReadOnlyList<Command> commands;

        public Bot()
        {
            try
            {
                bot = new VkApi();
                bot.Authorize(new ApiAuthParams() { AccessToken = ConfigMeneger.Configth.Token });
                try { Update(bot); } catch { }

                commands = GetCommand.GetCommands();


                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Version bot: {bot.VkApiVersion.Version}");
                Logger.Log($"[{DateTime.Now}] Version bot: {bot.VkApiVersion.Version}");

                Console.WriteLine("[The bot can start working]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][The bot can start working]");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(bot create)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(bot create)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(bot create)]: {ex.Message}", null);
            }
        }

        public void ReLoad_Conf()
        {
            try
            {
                bot.Authorize(new ApiAuthParams() { AccessToken = ConfigMeneger.Configth.Token });
                Update(bot);

                longPoll = bot.Groups.GetLongPollServer(ConfigMeneger.Configth.IdGroup);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[bot reloaded configth]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][bot reloaded configth]");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(bot reload configth)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(bot reload configth)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(bot reload configth)]: {ex.Message}", bot);
            }
        }

        public void Start()
        {
            try
            {
                if (IsActive == false)
                {
                    IsActive = true;

                    task = Task.Factory.StartNew(Working);

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("[Bot started work]");
                    Console.ResetColor();
                    Logger.Log($"[{DateTime.Now}][Bot started work]");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(bot start)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(bot start)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(bot start)]: {ex.Message}", bot);
            }
        }

        public void Stop()
        {
            try
            {
                if (IsActive == true)
                {
                    IsActive = false;

                    task.Wait();
                    task.Dispose();

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("[Bot stopped work]");
                    Console.ResetColor();
                    Logger.Log($"[{DateTime.Now}][Bot stopped work]");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(bot end)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(bot end)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(bot end)]: {ex.Message}", bot);
            }
        }

        private void Working()
        {
            try { longPoll = bot.Groups.GetLongPollServer(ConfigMeneger.Configth.IdGroup); }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(bot working(longPoll))]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(bot working(longPoll))]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(bot working(longPoll))]: {ex.Message}", bot);
            }

            while (IsActive == true)
            {
                try
                {
                    longPoll = bot.Groups.GetLongPollServer(ConfigMeneger.Configth.IdGroup);

                    string Ts = "";

                    if (ConfigMeneger.Configth.Ts != "-1") { Ts = ConfigMeneger.Configth.Ts; } else { Ts = longPoll.Ts; }

                    var history = bot.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams() { Server = longPoll.Server, Ts = Ts, Key = longPoll.Key, Wait = ConfigMeneger.Configth.Wait });

                    if (history?.Updates != null)
                    {
                        foreach (var message in history.Updates)
                        {
                            if (message.Type == GroupUpdateType.MessageNew)
                            {
                                if (message.MessageNew != null && message.MessageNew.Message != null && message.MessageNew.Message.Text != null && message.MessageNew.Message.PeerId != null && message.MessageNew.Message.Date != null)
                                {
                                    if (ConfigMeneger.Configth.IsWorkingSeeMessage == true) { Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine($"[{DateTime.Now}, {message.MessageNew.Message.Date}][{message.MessageNew.Message.PeerId.Value}]: {message.MessageNew.Message.Text}"); Console.ResetColor(); }
                                    Logger.Log($"[{DateTime.Now}, {message.MessageNew.Message.Date}][{message.MessageNew.Message.PeerId.Value}]: {message.MessageNew.Message.Text}");

                                    if (ConfigMeneger.Configth.IsActivateSayModule == true)
                                    {
                                        ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
                                        {
                                            WorkingSay(message);
                                        }), null);

                                    }

                                    if (ConfigMeneger.Configth.IsActivateCommentsModule == true)
                                    {
                                        ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
                                        {
                                            WorkingCommentsMessage(message);
                                        }), null);
                                    }
                                }
                            }
                        }
                    }

                    if (ConfigMeneger.Configth.Ts != "-1") { ConfigMeneger.Configth.Ts = "-1"; ConfigMeneger.SaveConfigth(); }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now}][exception(bot working)]: {ex.Message}");
                    Console.ResetColor();
                    Logger.Log($"[{DateTime.Now}][exception(bot working)]: {ex.Message}");
                    ExceptionMove.Exception($"[{DateTime.Now}][exception(bot working)]: {ex.Message}", null);
                }
            }

            ConfigMeneger.Configth.Ts = longPoll.Ts;
            ConfigMeneger.SaveConfigth();
        }

        private void WorkingSay(VkNet.Model.GroupUpdate.GroupUpdate message)
        {
            bool isActivated = false;

            if (ConfigMeneger.Configth.ActivateMessageSend == true)
            {

                if (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 49)
                {
                    if (DateTime.Now.Hour + 23 == ConfigMeneger.Configth.DateTime.Hour)
                    {
                        if (DateTime.Now.Minute + 50 == ConfigMeneger.Configth.DateTime.Minute)
                        {
                            if (DateTime.Now.Second == ConfigMeneger.Configth.DateTime.Second)
                            {
                                isActivated = SendMessageTime.Answer(message.MessageNew.Message.Text, message.MessageNew.Message.PeerId.Value);
                            }
                        }
                    }
                }
                else if (DateTime.Now.Hour == 23)
                {
                    if (DateTime.Now.Hour + 23 == ConfigMeneger.Configth.DateTime.Hour)
                    {
                        if (DateTime.Now.Minute + 10 == ConfigMeneger.Configth.DateTime.Minute)
                        {
                            if (DateTime.Now.Second == ConfigMeneger.Configth.DateTime.Second)
                            {
                                isActivated = SendMessageTime.Answer(message.MessageNew.Message.Text, message.MessageNew.Message.PeerId.Value);
                            }
                        }
                    }
                }
                else
                {
                    if (DateTime.Now.Hour == ConfigMeneger.Configth.DateTime.Hour)
                    {
                        if (DateTime.Now.Minute + 10 == ConfigMeneger.Configth.DateTime.Minute)
                        {
                            if (DateTime.Now.Second == ConfigMeneger.Configth.DateTime.Second)
                            {
                                isActivated = SendMessageTime.Answer(message.MessageNew.Message.Text, message.MessageNew.Message.PeerId.Value);
                            }
                        }
                    }
                }
            }

            if (isActivated == false)
            {
                foreach (var command in commands)
                {
                    if (command.Contains(message.MessageNew.Message.Text) == true)
                    {
                        if (ConfigMeneger.Configth.IsWorkingSeeMessage == true) { Console.ForegroundColor = ConsoleColor.DarkBlue; Console.WriteLine(command.Execute(message.MessageNew.Message, bot)); Console.ResetColor(); } else { command.Execute(message.MessageNew.Message, bot); }
                        isActivated = true;
                        break;
                    }
                }
            }


            if (isActivated == false && ConfigMeneger.Configth.IsActivateEhoBot == true)
            {
                if (ConfigMeneger.Configth.IsWorkingSeeMessage == true) { Console.ForegroundColor = ConsoleColor.DarkBlue; Console.WriteLine($"[{DateTime.Now}][Answer to {message.MessageNew.Message.PeerId.Value}]: {message.MessageNew.Message.Text}"); Console.ResetColor(); }
                Logger.Log($"[{DateTime.Now}][Answer to {message.MessageNew.Message.PeerId.Value}]: {message.MessageNew.Message.Text}");

                bot.Messages.Send(new MessagesSendParams() { UserId = message.MessageNew.Message.PeerId.Value, Message = message.MessageNew.Message.Text, RandomId = new Random().Next() });
            }
        }

        private void WorkingCommentsMessage(VkNet.Model.GroupUpdate.GroupUpdate message)
        {

        }

        private void WorkingCommentsWall()
        {

        }

        public void SendMessage(long id, string message)
        {
            try
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = id, Message = message, RandomId = new Random().Next() });

                if (ConfigMeneger.Configth.IsWorkingSeeMessage == true) { Console.ForegroundColor = ConsoleColor.Magenta; Console.WriteLine($"[{DateTime.Now}][Answer to {id}]: {message}"); Console.ResetColor(); }
                Logger.Log($"[{DateTime.Now}][Answer to {id}]: {message}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(Send message bot)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(Send message bot)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(Send message bot)]: {ex.Message}", bot);
            }
        }
    }
}
