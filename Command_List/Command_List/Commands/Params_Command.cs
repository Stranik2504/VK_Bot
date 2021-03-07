using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Command_List.Commands
{
    public class Params_Command : Admin_Command
    {
        public override string[] NameCommand => List("/params", "/parameters", "/парам", "/параметры");

        public override string NameClass => "Команда для изменения параметров(Configth)";

        public override string Explanation => "/params {Назание параметра} {Значение}";

        public override Access Access => Access.Proger;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 3)
            {
                string NameParams = message.Text.Split(' ')[1];

                bool notSaveConfigth = false;
                bool success = false;

                switch (NameParams.ToLower())
                {
                    case "token":
                        ConfigMeneger.Configth.Token = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "idgroup":
                        ConfigMeneger.Configth.IdGroup = (ulong)Convert.ToUInt64(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "wait":
                        ConfigMeneger.Configth.Wait = Convert.ToInt32(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "iduserid":
                        ConfigMeneger.Configth.IdUserId = Convert.ToInt32(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "idpoints":
                        ConfigMeneger.Configth.IdPoints = Convert.ToInt32(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "idpromocode":
                        ConfigMeneger.Configth.IdPromocode = Convert.ToInt32(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "iddiscount":
                        ConfigMeneger.Configth.IdDiscount = Convert.ToInt32(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "namepoints":
                        ConfigMeneger.Configth.NamePoints = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "namesave":
                        ConfigMeneger.Configth.NameSave = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "ts":
                        ConfigMeneger.Configth.Ts = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "account":
                        ConfigMeneger.Configth.Account = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "login":
                        ConfigMeneger.Configth.Login = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "hash":
                        ConfigMeneger.Configth.Hash = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "message":
                        ConfigMeneger.Configth.Message = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "getanswer":
                        ConfigMeneger.Configth.GetAnswer = message.Text.Split(' ')[2];
                        success = true;
                        break;
                    case "activatemessagesend":
                        ConfigMeneger.Configth.ActivateMessageSend = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "isworkingloging":
                        ConfigMeneger.Configth.IsWorkingLoging = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "isworkingseemessage":
                        ConfigMeneger.Configth.IsWorkingSeeMessage = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "parsenameadmin":
                        ConfigMeneger.Configth.ParseNameAdmin = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "isactivateexceptionmove":
                        ConfigMeneger.Configth.IsActivateExceptionMove = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "isregistretingquest":
                        ConfigMeneger.Configth.IsRegistretingQuest = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "isworkingquest":
                        ConfigMeneger.Configth.IsWorkingQuest = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "isstarton":
                        ConfigMeneger.Configth.IsStartOn = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "isstopon":
                        ConfigMeneger.Configth.IsStopOn = Convert.ToBoolean(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "datetime":
                        ConfigMeneger.Configth.DateTime = Convert.ToDateTime(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "timequest":
                        ConfigMeneger.Configth.TimeQuest = Convert.ToDateTime(message.Text.Split(' ')[2]);
                        success = true;
                        break;
                    case "convert":
                        Convertor(message.Text.Split(' ')[2], bot);
                        notSaveConfigth = true;
                        success = true;
                        break;
                }


                if (success == true)
                {
                    if (notSaveConfigth == false) { ConfigMeneger.SaveConfigth(); }

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Успешно", RandomId = new Random().Next() });

                    return "Success";
                }
                else
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Параметр не найден", RandomId = new Random().Next() });

                    return "Params not found";
                }
            }
            else
            {
                string answer = $"{Explanation} \n Token({ConfigMeneger.Configth.Token}); \n id group({ConfigMeneger.Configth.IdGroup}); \n wait({ConfigMeneger.Configth.Wait}); \n id userid({ConfigMeneger.Configth.IdUserId}); \n id points({ConfigMeneger.Configth.IdPoints}); \n id promocode({ConfigMeneger.Configth.IdPromocode}); \n id discount({ConfigMeneger.Configth.IdDiscount}); \n name points({ConfigMeneger.Configth.NamePoints}); \n name save({ConfigMeneger.Configth.NameSave}); \n ts({ConfigMeneger.Configth.Ts}); \n account({ConfigMeneger.Configth.Account}); \n login({ConfigMeneger.Configth.Login}); \n hash({ConfigMeneger.Configth.Hash}); \n message({ConfigMeneger.Configth.Message}); \n get answer({ConfigMeneger.Configth.GetAnswer}); \n activate message send({ConfigMeneger.Configth.ActivateMessageSend}); \n is working loging({ConfigMeneger.Configth.IsWorkingLoging}); \n is working see message({ConfigMeneger.Configth.IsWorkingSeeMessage}); \n parse name admin({ConfigMeneger.Configth.ParseNameAdmin}); \n is activate exceptoin move({ConfigMeneger.Configth.IsActivateExceptionMove}); \n isregistreting quest({ConfigMeneger.Configth.IsRegistretingQuest})' \n isworking quest({ConfigMeneger.Configth.IsWorkingQuest}); \n isstarton({ConfigMeneger.Configth.IsStartOn}); \n isstopon({ConfigMeneger.Configth.IsStopOn}); \n date time ({ConfigMeneger.Configth.DateTime}); \n time quest({ConfigMeneger.Configth.TimeQuest});";

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = answer, RandomId = new Random().Next() });

                return answer;
            }
        }

        private void Convertor(string tag, VkApi bot)
        {
            switch (tag.ToLower())
            {
                case "listtodatabase":
                    List(bot);
                    break;
                case "databasetojson":
                    Database(true, bot);
                    break;
                case "databasetoxml":
                    Database(false, bot);
                    break;
            }
        }

        private void ClearBase(VkApi bot)
        {
            using (SqlConnection connection = new SqlConnection(stringBuilder.ConnectionString))
            {
                connection.Open();

                List<string> ids = new List<string>();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Admins", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(reader["UserId"].ToString());
                        }
                    }
                }

                foreach (var id in ids)
                {
                    using (SqlCommand command = new SqlCommand($"DELETE FROM Admins WHERE UserId = '{id}'", connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in ClearBase");
                            ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in ClearBase", bot);
                        }
                    }
                }
            }
        }

        private void List(VkApi bot)
        {
            using (SqlConnection connection = new SqlConnection(stringBuilder.ConnectionString))
            {
                connection.Open();

                ClearBase(bot);

                foreach (var admin in AdminsList.Admins)
                {
                    using (SqlCommand command = new SqlCommand($"INSERT INTO Admins (Name, UserId, Access) VALUES ('{admin.Name}', {admin.UserId}, {admin.Access})", connection))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in ConvertXmlToDatabase");
                            ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in ConvertXmlToDatabase", bot);
                        }
                    }
                }
            }
        }

        private void Database(bool IsJson, VkApi bot)
        {
            using (SqlConnection connection = new SqlConnection(stringBuilder.ConnectionString))
            {
                connection.Open();

                AdminsList.CreateAdmins();

                try
                {
                    using (SqlCommand command = new SqlCommand("SELECT * FROM Admins", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AdminsList.Admins.Add(new Admin() { Name = reader["Name"].ToString(), UserId = (long)Convert.ToInt64(reader["UserId"].ToString()), Access = Convert.ToInt32(reader["Access"].ToString()) });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in ConvertDatebaseToList");
                    ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in ConvertDatebaseToList", bot);
                }

                if (IsJson == true) { AdminsList.SaveJsonListAdmins(); } else { AdminsList.SaveXmlListAdmins(); }
            }
        }
    }
}
