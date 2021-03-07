using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Data.SqlClient;

namespace Command_List.Commands
{
    public class ReAccess_Admin_Command : Admin_Command
    {

        public override string[] NameCommand => List("/reaccess", "/readmin", "/реадмин");

        public override string NameClass => "Команда для смены доступа у админов";

        public override string Explanation => "/reaccess {User id} {Доступ}";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 3)
            {
                if (message.Text.Split(' ')[1] != null && message.Text.Split(' ')[1] != "" && message.Text.Split(' ')[1] != " ")
                {
                    int.TryParse(message.Text.Split(' ')[1], out int UserId);
                    int.TryParse(message.Text.Split(' ')[2], out int AccessLevel);

                    string mess = ReAccess(UserId, numberAccess, AccessLevel, bot);

                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = mess, RandomId = new Random().Next() });

                    return mess;
                }
                else
                {
                    return "Error";
                }
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = Explanation, RandomId = new Random().Next() });

                return Explanation;
            }
        }

        private string ReAccess(long UserId, int AccessLevel, int NeedAccessLevel, VkApi bot)
        {
            switch (ConfigMeneger.Configth.NameSave.ToLower())
            {
                case "database":
                    return Database(UserId, AccessLevel, NeedAccessLevel, bot);
                case "xml":
                    return XmlAndJson(UserId, AccessLevel, NeedAccessLevel, false, bot);
                case "json":
                    return XmlAndJson(UserId, AccessLevel, NeedAccessLevel, true, bot);
                default:
                    return $"Ошибка: невозможно обновить уровень доступа для юзера {UserId} в базе админов";
            }
        }

        private string Database(long UserId, int AccessLevel, int NeedAccessLevel, VkApi bot)
        {
            using (SqlConnection connection = new SqlConnection(stringBuilder.ConnectionString))
            {
                connection.Open();

                int AdminAccess = CheakAccess(UserId, bot);

                if (AdminAccess != -1 && AdminAccess < Convert.ToInt32(Access.User))
                {
                    if (AccessLevel < AdminAccess && NeedAccessLevel >= AccessLevel)
                    {
                        using (SqlCommand command = new SqlCommand($"UPDATE Admins SET Access = '{NeedAccessLevel}' WHERE UserId = '{UserId}'", connection))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in ReAccess");
                                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in ReAccess", bot);
                                return $"Ошибка: невозможно обновить уровень доступа для юзера {UserId} в базе админов";
                            }
                        }

                        return $"Юзер {UserId} получил новый уровень доступа в базе админов";
                    }
                    else
                    {
                        return $"У тебя нет разрешения";
                    }
                }
                else
                {
                    return $"Юзера {UserId} нет в базе админов";
                }
            }
        }

        private string XmlAndJson(long UserId, int AccessLevel, int NeedAccessLevel, bool IsJson, VkApi bot)
        {
            int AdminAccess = CheakAccess(UserId, bot);

            if (AdminAccess != -1 && AdminAccess < Convert.ToInt32(Access.User))
            {
                if (AccessLevel < AdminAccess && NeedAccessLevel >= AccessLevel)
                {
                    bool reAccess = false;

                    foreach (var admin in AdminsList.Admins)
                    {
                        if (admin.UserId == UserId)
                        {
                            admin.Access = NeedAccessLevel;
                            reAccess = true;
                            break;
                        }
                    }

                    if (reAccess == true)
                    {
                        if (IsJson == true) { AdminsList.SaveJsonListAdmins(); } else { AdminsList.SaveXmlListAdmins(); }
                        return $"Юзер {UserId} получил новый уровень доступа в базе админов";
                    }
                    else
                    {
                        return $"Юзера {UserId} нет в базе админов";
                    }
                }
                else
                {
                    return $"У тебя нет разрешения";
                }
            }
            else
            {
                return $"Юзера {UserId} нет в базе админов";
            }
        }
    }
}
