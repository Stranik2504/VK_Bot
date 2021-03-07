using System;
using VkNet;
using VkNet.Model;
using System.Data.SqlClient;
using Classes;

namespace Command_List
{
    public abstract class Admin_Command : Command
    {
        protected SqlConnectionStringBuilder stringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = @".\SQLEXPRESS",
            AttachDBFilename = "|DataDirectory|Admins.mdf",
            IntegratedSecurity = true,
            ConnectTimeout = 30,
            UserInstance = true
        };

        protected int numberAccess = Convert.ToInt32(Access.User);

        protected int CheakAccess(long UserId, VkApi bot)
        {
            switch (ConfigMeneger.Configth.NameSave.ToLower())
            {
                case "database":
                    return DateBase(UserId, bot);
                case "xml":
                    return XmlAndJson(UserId);
                case "json":
                    return XmlAndJson(UserId);
                default:
                    return Convert.ToInt32(Access.User);
            }
        }

        protected void CheakAccessAdmin(long UserId, VkApi bot)
        {
            switch (ConfigMeneger.Configth.NameSave.ToLower())
            {
                case "database":
                    numberAccess = DateBase(UserId, bot);
                    break;
                case "xml":
                    numberAccess = XmlAndJson(UserId);
                    break;
                case "json":
                    numberAccess = XmlAndJson(UserId);
                    break;
                default:
                    numberAccess = Convert.ToInt32(Access.User);
                    break;
            }
        }

        private int DateBase(long UserId, VkApi bot)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(stringBuilder.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand($"SELECT * FROM Admins WHERE UserId = '{UserId}'", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            int.TryParse(reader["Access"].ToString(), out int accessNumber);
                            return accessNumber;
                        }
                    }
                }
            }
            catch (Exception ex) { ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}", bot); return Convert.ToInt32(Access.User); }
        }

        private int XmlAndJson(long UserId)
        {
            foreach (var admin in AdminsList.Admins)
            {
                if (admin.UserId == UserId)
                {
                    return admin.Access;
                }
            }

            return Convert.ToInt32(Access.User);
        }

        public override string Execute(Message message, VkApi bot)
        {
            try
            {
                if (message != null && bot != null && message.PeerId != null)
                {
                    CheakAccessAdmin(message.PeerId.Value, bot);
                    string output = "";

                    if ((numberAccess <= Convert.ToInt32(Access)) && (numberAccess >= 0)) { output = Move(message, bot); }
                    else
                    {
                        Logger.Log($"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error: Data entered incorrectly(This user {message.PeerId.Value} connot using this command)");
                        return $"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error: Data entered incorrectly(This user {message.PeerId.Value} connot using this command)";
                    }

                    Logger.Log($"[{DateTime.Now}][Answer to {message.PeerId.Value}][{NameClass}]: {output}");
                    return $"[{DateTime.Now}][Answer to {message.PeerId.Value}][{NameClass}]: {output}";
                }
                else
                {
                    Logger.Log($"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error");
                    return $"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error";
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}", bot);
                return $"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}";
            }
        }
    }
}
