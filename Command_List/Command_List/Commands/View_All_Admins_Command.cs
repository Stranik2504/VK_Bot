using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Command_List.Commands
{
    public class View_All_Admins_Command : Admin_Command
    {
        public override string[] NameCommand => List("/view", "/admins", "/админы");

        public override string NameClass => "Команда для получения всех админов";

        public override string Explanation => "/view";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            string answer = GetAllAdmins(numberAccess, bot);

            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = answer, RandomId = new Random().Next() });

            return answer;
        }

        private string GetAllAdmins(int MyAccess, VkApi bot)
        {
            if (ConfigMeneger.Configth.NameSave.ToLower() == "database")
            {
                using (SqlConnection connection = new SqlConnection(stringBuilder.ConnectionString))
                {
                    connection.Open();
                    string answer = "";

                    try
                    {
                        using (SqlCommand command = new SqlCommand("SELECT * FROM Admins", connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (MyAccess >= 0 && MyAccess <= Convert.ToInt32(reader["Access"])) { answer += $"Имя: {reader["Name"]}(UserId: {reader["UserID"]}); Доступ: {reader["Access"]} \n"; }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}", bot); }

                    return answer;
                }
            }
            else
            {
                string answer = "";

                foreach (var admin in AdminsList.Admins)
                {
                    if (MyAccess >= 0 && MyAccess <= Convert.ToInt32(admin.Access)) { answer += $"Имя: {admin.Name}(UserId: {admin.UserId}); Доступ: {admin.Access} \n"; }
                }

                return answer;
            }
        }
    }
}
