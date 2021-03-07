using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;

namespace Command_List.Commands
{
    public class Clear_Command : Admin_Command
    {
        public override string[] NameCommand => List("/clear");

        public override string NameClass => "Команада для отчистки файлов";

        public override string Explanation => "/clear {Назание файла}";

        public override Access Access => Access.SuperAdmin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 2)
            {
                string NameParams = message.Text.Split(' ')[1];

                bool success = false;

                switch (NameParams.ToLower())
                {
                    case "peoples":
                        PeopleList.Peoples.Clear();
                        PeopleList.SavePeople();
                        success = true;
                        break;
                    case "admins":
                        AdminsList.Admins.Clear();
                        if (ConfigMeneger.Configth.NameSave.ToLower() == "json") { AdminsList.SaveJsonListAdmins(); } else if (ConfigMeneger.Configth.NameSave.ToLower() == "xml") { AdminsList.SaveXmlListAdmins(); }
                        success = true;
                        break;
                    case "register":
                        RegisterList.Users.Clear();
                        RegisterList.SaveListUser();
                        success = true;
                        break;
                    case "questions":
                        QuestionsList.Questions.Clear();
                        QuestionsList.SaveQuestions();
                        success = true;
                        break;
                    case "log":
                        StreamWriter writer = new StreamWriter("LogBot.log");
                        writer.Close();
                        writer.Dispose();
                        success = true;
                        break;
                }


                if (success == true)
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Успешно", RandomId = new Random().Next() });

                    return "Success";
                }
                else
                {
                    bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Файл не найден", RandomId = new Random().Next() });

                    return "File not found";
                }
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Файлы: peoples, admins, register, questions, log", RandomId = new Random().Next() });

                return "Files: configth, peoples, admins, register list, log";
            }
        }
    }
}
