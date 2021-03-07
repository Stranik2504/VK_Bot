using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VK_Bot.Components.Commands.ACoins;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

using static System.Diagnostics.Debug;

namespace VK_Bot.Components
{
    public class Bot
    {
        public bool IsActive { get; private set; } = false;

        private static VkApi _bot = new VkApi();
        private static VkApi _user = new VkApi();
        private static LongPollServerResponse _longPollServer = null;

        private bool _isFirstError = true;
        private string DatePoints;

        public static bool TrySendTriggerAdmins(string message, string payload)
        {
            try
            {
                foreach (var admin in Database.GetCells(Place.Admins))
                {
                    if (admin.Fields.ContainsKey("TriggerByAdd") ? (bool.Parse(admin.Fields["TriggerByAdd"].ToString()) ? true : false) : false)
                    {
                        if (message != "")
                        {
                            if (payload != "" && payload != null) { _bot.Messages.Send(new MessagesSendParams() { UserId = admin.Fields["UserId"].ToString().ToLong(), Message = message, Payload = "{\"acion\":\"" + payload + "\"}", RandomId = 0 }); }
                            else { _bot.Messages.Send(new MessagesSendParams() { UserId = admin.Fields["UserId"].ToString().ToLong(), Message = message, RandomId = 0 }); }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex) { $"[Bot][TrySendTriggerAdmins]: {ex.Message}".Log(); }

            return false;
        }

        public static bool TrySendUser(long userId, string message, string payload)
        {
            try
            {
                MessageKeyboard keyboard = CreateButton(_user.Users.Get(new long[] { userId }, ProfileFields.Domain, NameCase.Nom)[0].Domain);

                if (message != "")
                {
                    if (keyboard != null)
                    {
                        if (payload != "" && payload != null) { _bot.Messages.Send(new MessagesSendParams() { UserId = userId, Message = message, Keyboard = keyboard, Payload = "{\"acion\":\"" + payload + "\"}", RandomId = 0 }); }
                        else { _bot.Messages.Send(new MessagesSendParams() { UserId = userId, Message = message, Keyboard = keyboard, RandomId = 0 }); }
                    }
                    else
                    {
                        if (payload != "" && payload != null) { _bot.Messages.Send(new MessagesSendParams() { UserId = userId, Message = message, Payload = "{\"acion\":\"" + payload + "\"}", RandomId = 0 }); }
                        else { _bot.Messages.Send(new MessagesSendParams() { UserId = userId, Message = message, RandomId = 0 }); }
                    }
                }

                return true;
            }
            catch (Exception ex) { $"[Bot][TrySendUser]: {ex.Message}".Log(); }

            return false;
        }

        public static string GetNameUser(long userId)
        {
            var user = _user.Users.Get(new long[] { userId }, ProfileFields.Domain, NameCase.Nom)[0];

            if ((user.FirstName != null ? user.FirstName + " " : "") + (user.FirstName != null ? user.LastName : "") == "") { return ""; }

            return (user.FirstName != null ? user.FirstName + " " : "") + (user.FirstName != null ? user.LastName : "");
        }

        public void ReloadParams()
        {
            try
            {
                _bot.Authorize(new ApiAuthParams() { AccessToken = ConfigManager.Configs.Token });
                _user.Authorize(new ApiAuthParams() { AccessToken = ConfigManager.Configs.UserToken });
                _longPollServer = _bot.Groups.GetLongPollServer((ulong)ConfigManager.Configs.IdGroup);
            }
            catch (Exception ex) { $"[Bot][ReloadParams]: {ex.Message}".Log(); }
        }

        public void StartBot()
        {
            try
            {
                if (IsActive == false)
                {
                    IsActive = true;
                    DatePoints = DateTime.Now.ToLongDateString();
                    new Thread(Move).Start();
                }
            }
            catch (Exception ex) { $"[Bot][StartBot]: {ex.Message}".Log(); }
        }

        public void StopBot()
        {
            try { IsActive = false; } catch (Exception ex) { $"[Bot][StopBot]: {ex.Message}".Log(); }
        }

        private static MessageKeyboard CreateButton(string domain)
        {
            KeyboardBuilder keys = new KeyboardBuilder();
            bool isHaveButton = false;

            if (ConfigManager.Configs.IsActivateRaffle)
            {
                keys.AddButton(new MessageKeyboardButtonAction() { Label = "Информация", Payload = "{\"action\":\"/help\"}", Type = KeyboardButtonActionType.Text }, KeyboardButtonColor.Primary);
                keys.AddButton(new MessageKeyboardButtonAction() { Label = "Количество поинтов", Payload = "{\"action\":\"/points\"}", Type = KeyboardButtonActionType.Text }, KeyboardButtonColor.Positive);
                keys.AddLine();
                keys.AddButton(new MessageKeyboardButtonAction() { Label = "Посмотреть команды", Payload = "{\"action\":\"/commands\"}", Type = KeyboardButtonActionType.Text }, KeyboardButtonColor.Negative);
                isHaveButton = true;
            }

            if (ConfigManager.Configs.IsActivateACoinsButton || ConfigManager.Configs.IsActivateACoins)
            {
                if (Database.IsInDatabase(Place.ClubCard, domain))
                {
                    if (ConfigManager.Configs.IsActivateACoinsButton && !ConfigManager.Configs.IsActivateACoins) { keys.AddLine(); }
                    keys.AddButton(new MessageKeyboardButtonAction() { Label = "Количество ACoin", Payload = "{\"acion\":\"/acoins\"}", Type = KeyboardButtonActionType.Text }, KeyboardButtonColor.Primary);
                    keys.AddButton(new MessageKeyboardButtonAction() { Label = "Мой промокод", Payload = "{\"action\":\"/promocode\"}", Type = KeyboardButtonActionType.Text }, KeyboardButtonColor.Positive);
                }
                else
                {
                    if (ConfigManager.Configs.IsActivateACoinsButton && !ConfigManager.Configs.IsActivateACoins) { keys.AddLine(); }
                    keys.AddButton(new MessageKeyboardButtonAction() { Label = "Зарегистрироваться", Payload = "{\"acion\":\"/acoins_registration\"}", Type = KeyboardButtonActionType.Text }, KeyboardButtonColor.Positive);
                }

                isHaveButton = true;
            }

            if (isHaveButton) { return keys.Build(); }

            return null;
        }

        private void Move()
        {
            while (IsActive == true)
            {
                try
                {
                    if (ConfigManager.Configs.Ts == -1) { ConfigManager.Configs.Ts = long.Parse(_longPollServer.Ts); }
                    if (ConfigManager.Configs.IsActivateRaffle && DatePoints != DateTime.Now.ToLongDateString()) { DatePoints = DateTime.Now.ToLongDateString(); AddPointsForRepost(); }
                    _longPollServer = _bot.Groups.GetLongPollServer((ulong)ConfigManager.Configs.IdGroup);

                    var history = _bot.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams() { Server = _longPollServer.Server, Ts = ConfigManager.Configs.Ts.ToString(), Key = _longPollServer.Key, Wait = 25 });

                    foreach (var update in history?.Updates)
                    {

                        try
                        {
                            if (update.Type == GroupUpdateType.MessageNew || update.Type == GroupUpdateType.MessageEdit)
                            {
                                if (update.MessageNew != null && update.MessageNew.Message.Text != null && update.MessageNew.Message.PeerId != null)
                                {
                                    ThreadPool.QueueUserWorkItem(new WaitCallback((x) => { Correspond(update.MessageNew); }), null);
                                }
                            }

                            if (update.Type == GroupUpdateType.WallReplyNew)
                            {
                                if (ConfigManager.Configs.IsActivateRaffle || ConfigManager.Configs.TestersIds.Contains(update.WallReply.FromId.Value))
                                {
                                    ThreadPool.QueueUserWorkItem(new WaitCallback((x) => { SendAnswerComment(update.WallReply); }), null);
                                }
                            }

                            if (update.Type == GroupUpdateType.WallRepost)
                            {
                                if (ConfigManager.Configs.IsActivateRaffle || ConfigManager.Configs.TestersIds.Contains(update.WallReply.FromId.Value))
                                {
                                    ThreadPool.QueueUserWorkItem(new WaitCallback((x) => { Repost(update.WallPost); }), null);
                                }
                            }

                            ConfigManager.Configs.Ts++;
                        }
                        catch (Exception ex) { $"[Bot][Move][foreach]: {ex.Message}".Log(); throw; }
                    }

                    ConfigManager.SaveConfig();
                    _isFirstError = true;
                }
                catch (Exception ex) { if (_isFirstError) { DatePoints = DateTime.Now.ToLongDateString(); ReloadParams(); _isFirstError = false; } else { $"[Bot][Move]: {ex.Message}".Log(); break; } }
            }
        }      

        private void Correspond(MessageNew message)
        {
            try
            {
                MessageKeyboard keyboard = null;
                Output answer = new Output("");
                Dictionary<Additions, string> additions = new Dictionary<Additions, string>();

                string uri = "";
                if (message.Message.Attachments.Count > 0)
                {
                    if (message.Message.Attachments.First().Instance is Photo) { var photo = message.Message.Attachments.First().Instance as Photo; uri = photo.Sizes[photo.Sizes.Count - 1].Url.AbsoluteUri; }
                    if (message.Message.Attachments.First().Instance is Document) { uri = (message.Message.Attachments.First().Instance as Document).Uri; }
                }

                var domain = _user.Users.Get(new long[] { message.Message.PeerId.Value }, ProfileFields.Domain, NameCase.Nom)[0].Domain;

                additions.Add(Additions.Attachments, uri);
                additions.Add(Additions.Domain, domain);
                additions.Add(Additions.UserId, message.Message.PeerId.Value.ToString());
                
                if (message.Message.Text.StartsWith("/"))
                {
                    answer = Find_Commands.Execute(message.Message.Text, additions);
                }
                else if (message.Message.Payload != null && message.Message.Payload.Split('"')[3].StartsWith("\\") && message.Message.Payload.Split('"')[3].Remove(0, 1).StartsWith("/"))
                {
                    answer = Find_Commands.Execute(message.Message.Payload.Split('"')[3].Remove(0, 1), additions);
                }
                else if (message.Message.Payload != null && message.Message.Payload.Split('"')[3].StartsWith("/"))
                {
                    answer = Find_Commands.Execute(message.Message.Payload.Split('"')[3], additions);
                }
                else if (message.Message.ReplyMessage != null && message.Message.ReplyMessage.Payload.Split('"')[3].StartsWith("\\") && message.Message.ReplyMessage.Payload.Split('"')[3].Remove(0, 1).StartsWith("/"))
                {
                    additions[Additions.ReplyUserId] = message.Message.ReplyMessage.FromId.Value.ToString();
                    answer = Find_Commands.Execute(message.Message.ReplyMessage.Payload.Split('"')[3].Remove(0, 1) + " " + message.Message.Text, additions);
                }
                else
                {
                    if (Database.IsInDatabase(Place.ClubCard, domain))
                    {
                        if (ConfigManager.Configs.IsActivateACoins) { answer = "Привет! Для получения количества ACoins, нажмите на кнопку или введите команду /acoins. Для получения промокода также нажмите на кнопку, или введите команду /promocode.".ToOutput(); }
                        else if (ConfigManager.Configs.IsActivateRaffle) { answer = new Commands.Prize.Info_Command().Move("", null); }
                    }
                    else { answer = new ACoins_Info_Registration_Command().Move("", null); }
                }

                keyboard = CreateButton(domain);

                if (answer != null && answer.Answer != "")
                {
                    if (keyboard != null)
                    {
                        if (answer is OPayload && ((answer as OPayload).Payload != "" || (answer as OPayload) != null)) { _bot.Messages.Send(new MessagesSendParams() { UserId = message.Message.PeerId.Value, Keyboard = keyboard, Message = answer.Answer, Payload = "{\"acion\":\"" + (answer as OPayload).Payload + "\"}", RandomId = 0 }); }
                        else { _bot.Messages.Send(new MessagesSendParams() { UserId = message.Message.PeerId.Value, Keyboard = keyboard, Message = answer.Answer, RandomId = 0 }); }
                    }
                    else
                    {
                        if (answer is OPayload && ((answer as OPayload).Payload != "" || (answer as OPayload) != null)) { _bot.Messages.Send(new MessagesSendParams() { UserId = message.Message.PeerId.Value, Message = answer.Answer, Payload = "{\"acion\":\"" + (answer as OPayload).Payload + "\"}", RandomId = 0 }); }
                        else { _bot.Messages.Send(new MessagesSendParams() { UserId = message.Message.PeerId.Value, Message = answer.Answer, RandomId = 0 }); }
                    }
                }
            }
            catch (Exception ex) { $"[Bot][Correspond]: {ex.Message}".Log(); throw; }
        }

        private void Repost(WallPost post)
        {
            try
            {
                if (Database.IsInDatabase(Place.Users, post.FromId.Value.ToString()))
                {
                    if (Database.GetValueData<bool>(Place.Users, post.FromId.Value.ToString(), nameSearchField: "FirstReposted").Field == false)
                    {
                        Database.Log(-1, post.FromId.Value, ConfigManager.Configs.PostPoints, "first post add");
                        Database.SetValueData(Place.Users, post.FromId.Value.ToString(), "FirstReposted", true);
                        Database.SetValueData(Place.Users, post.FromId.Value.ToString(), "Points", Database.GetValueData<long>(Place.Users, post.FromId.Value.ToString(), nameSearchField: "Points").Field + ConfigManager.Configs.PostPoints);
                    }
                }
            }
            catch (Exception ex) { $"[Bot][Repost]: {ex.Message}".Log(); }
        }

        private void AddPointsForRepost()
        {
            try
            {
                while (IsActive)
                {
                    if (DateTime.Now.Second > 0 && DateTime.Now.Second < 1 && DateTime.Now.Minute == 0 && DateTime.Now.Hour == 0)
                    {
                        var allpostscount = _user.Wall.Get(new WallGetParams() { Count = 1, OwnerId = -ConfigManager.Configs.IdGroup }).TotalCount;

                        for (ulong i = 0; i < (allpostscount % 2 == 1 ? allpostscount / 100 + 1 : allpostscount / 100); i++)
                        {
                            var post = _user.Wall.Get(new WallGetParams() { Count = 100, Offset = 100 * i, OwnerId = -ConfigManager.Configs.IdGroup }).WallPosts.Where(x => x.Id == ConfigManager.Configs.IdPost)?.First();

                            if (post != null)
                            {
                                try
                                {
                                    var allrepostscount = _user.Wall.GetReposts(-ConfigManager.Configs.IdGroup, ConfigManager.Configs.IdPost, 0, 100).TotalCount;
                                    for (ulong j = 0; j < (allpostscount % 2 == 1 ? allpostscount / 100 + 1 : allpostscount / 100); j++)
                                    {
                                        foreach (var item2 in _user.Wall.GetReposts(-ConfigManager.Configs.IdGroup, ConfigManager.Configs.IdPost, (long)(100 * j), 100).WallPosts)
                                        {
                                            if (Database.IsInDatabase(Place.Users, item2.FromId.Value.ToString()))
                                            {
                                                Database.Log(-1, item2.FromId.Value, ConfigManager.Configs.PostPoints, "repost add at 0:0:0");
                                                Database.SetValueData(Place.Users, item2.FromId.Value.ToString(), "Points", Database.GetValueData<long>(Place.Users, item2.FromId.Value.ToString(), nameSearchField: "Points").Field + ConfigManager.Configs.PostPoints);
                                            }
                                        }
                                    }
                                }
                                catch { throw; }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { $"[Bot][AddPointsForRepost]: {ex.Message}".Log(); }
        }

        private void SendAnswerComment(VkNet.Model.GroupUpdate.WallReply wallReply)
        {
            try
            {
                if (ConfigManager.Configs.IdPost == wallReply.PostId.Value && (wallReply.Text.ToLower() == $"[club{ConfigManager.Configs.IdGroup}|{ConfigManager.Configs.NameGroup}],".ToLower() + ConfigManager.Configs.KeyWord.ToLower() || wallReply.Text.ToLower() == $"[club{ConfigManager.Configs.IdGroup}|{ConfigManager.Configs.NameGroup}], ".ToLower() + ConfigManager.Configs.KeyWord.ToLower() || wallReply.Text.ToLower() == ConfigManager.Configs.KeyWord.ToLower()))
                {
                    if (Database.IsInDatabase(Place.Users, wallReply.FromId.Value.ToString()) == false)
                    {
                        _bot.Wall.CreateComment(new WallCreateCommentParams() { PostId = wallReply.PostId.Value, OwnerId = -ConfigManager.Configs.IdGroup, ReplyToComment = wallReply.Id, Message = $"Вы не зарегистрированы. Для регистрации, личные в сообщения сообщества, нажмите кнопку: \"Начать\", или отправьте сообщение: \"/start\" или \"/начать\"" });
                    }
                    else
                    {
                        long points = Database.GetValueData<long>(Place.Users, wallReply.FromId.Value.ToString(), nameSearchField: "Points").Field;

                        if (points > 0)
                        {
                            // Определение радндомного подарка
                            var prize = Database.GetRandomPrize();

                            // Создание текстуры для приза
                            try { using (WebClient client = new WebClient()) { client.DownloadFile(new Uri(_bot.Users.Get(new long[] { wallReply.FromId.Value }, ProfileFields.Photo100, NameCase.Nom).First().Photo100.ToString()), $@"Avatar\{wallReply.FromId.Value}.jpg"); } } catch (Exception ex) { $"[Bot][CreateTexture][DownloadAvatar]: {ex.Message}".Log(); }
                            var user = _bot.Users.Get(new long[] { wallReply.FromId.Value }).First();

                            string filename = ImageOverlay.CreateTexture(wallReply.FromId.Value.ToString(), user.FirstName, prize.Photos);

                            // Добавление фотографии на сервер для дальнейшей отправки в сообщениях юзеру
                            var photo = _user.Photo.SaveMessagesPhoto(Encoding.ASCII.GetString(new WebClient().UploadFile(_user.Photo.GetMessagesUploadServer(ConfigManager.Configs.IdGroup).UploadUrl, filename)));
                            var photo_comment = _user.Photo.SaveWallPhoto(Encoding.ASCII.GetString(new WebClient().UploadFile(_user.Photo.GetWallUploadServer(ConfigManager.Configs.IdGroup).UploadUrl, filename)), 0, (ulong)ConfigManager.Configs.IdGroup);

                            // Ответ в коментариях
                            if (prize.IsWin == true) { _bot.Wall.CreateComment(new WallCreateCommentParams() { PostId = wallReply.PostId.Value, OwnerId = -ConfigManager.Configs.IdGroup, ReplyToComment = wallReply.Id, Message = ConfigManager.Configs.AnswerWinTextComment, Attachments = photo_comment }); }
                            else { _bot.Wall.CreateComment(new WallCreateCommentParams() { PostId = wallReply.PostId.Value, OwnerId = -ConfigManager.Configs.IdGroup, ReplyToComment = wallReply.Id, Message = ConfigManager.Configs.AnswerLoseTextComment, Attachments = photo_comment }); }

                            // Ответ в личных сообщениях
                            if (prize.Text != "") { _bot.Messages.Send(new MessagesSendParams() { UserId = wallReply.FromId.Value, RandomId = 0, Message = prize.Text, Attachments = photo }); }

                            // Уменьшение количества поинтов
                            Database.Log(-1, wallReply.FromId.Value, -1, "get prize reduce");
                            Database.SetValueData(Place.Users, wallReply.FromId.Value.ToString(), "Points", points - 1);

                            // Удаление файл, после отправки
                            if (File.Exists(filename)) { File.Delete(filename); }
                        }
                        else
                        {
                            _bot.Wall.CreateComment(new WallCreateCommentParams() { PostId = wallReply.PostId.Value, OwnerId = -ConfigManager.Configs.IdGroup, ReplyToComment = wallReply.Id, Message = $"У вас недостаточно баллов. Ты можешь получить баллы: При старте ты получаешь поинты({ConfigManager.Configs.StartPoints}). Ты можешь получить поинт за репост в размере {ConfigManager.Configs.PostPoints} и за каждый день после, ты будешь получать столько же. Так же ты можешь поиграть с ботом на баллы" });
                        }
                    }
                }
            }
            catch (Exception ex) { $"[Bot][SendAnswerComment]: {ex.Message}".Log(); }
        }
    }

}
