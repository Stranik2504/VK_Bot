using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Data.SqlClient;

namespace Command_List.Commands
{
    public class Delete_Admin_Command : Admin_Command
    {
        public override string[] NameCommand => List("/delete_adm", "/удалить_адм");

        public override string NameClass => "Команда для удаления админа";

        public override string Explanation => "/delete_adm {User id}";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 2)
            {
                if (message.Text.Split(' ')[1] != null && message.Text.Split(' ')[1] != "" && message.Text.Split(' ')[1] != " ")
                {
                    int.TryParse(message.Text.Split(' ')[1], out int UserId);

                    string mess = RemoveFromBase(UserId, numberAccess, bot);

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

        private string RemoveFromBase(long UserId, int AccessLevel, VkApi bot)
        {
            switch (ConfigMeneger.Configth.NameSave.ToLower())
            {
                case "database":
                    return DataBase(UserId, AccessLevel, bot);
                case "xml":
                    return XmlAndJson(UserId, AccessLevel, false, bot);
                case "json":
                    return XmlAndJson(UserId, AccessLevel, true, bot);
                default:
                    Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: Error: can't deleted user {UserId} from base");
                    return $"Ошибка: невозможно удалить юзера {UserId} из базы админов";
            }
        }

        private string DataBase(long UserId, int AccessLevel, VkApi bot)
        {
            using (SqlConnection connection = new SqlConnection(stringBuilder.ConnectionString))
            {
                connection.Open();

                int AdminAccess = CheakAccess(UserId, bot);

                if (AdminAccess != -1 && AdminAccess < Convert.ToInt32(Access.User))
                {
                    if (AccessLevel < AdminAccess)
                    {
                        using (SqlCommand command = new SqlCommand($"DELETE FROM Admins WHERE UserId = '{UserId}'", connection))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in RemoveFromBase");
                                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in RemoveFromBase", bot);
                                return $"Ошибка: невозможно удалить юзера {UserId} из базы админов";
                            }
                        }

                        return $"Юзер {UserId} удален из базы админов";
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

        private string XmlAndJson(long UserId, int AccessLevel, bool IsJson, VkApi bot)
        {
            int AdminAccess = CheakAccess(UserId, bot);

            if (AdminAccess != -1 && AdminAccess < Convert.ToInt32(Access.User))
            {
                if (AccessLevel < AdminAccess)
                {
                    bool deleted = false;

                    for (int i = 0; i < AdminsList.Admins.Count; i++)
                    {
                        if (AdminsList.Admins[i].UserId == UserId)
                        {
                            AdminsList.Admins.RemoveAt(i);
                            deleted = true;
                            break;
                        }
                    }

                    if (deleted == true)
                    {
                        if (IsJson == true) { AdminsList.SaveJsonListAdmins(); } else { AdminsList.SaveXmlListAdmins(); }

                        return $"Юзер {UserId} удален из базы админов";
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
