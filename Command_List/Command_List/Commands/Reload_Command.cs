using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Command_List.Commands
{
    public class Reload_Command : Admin_Command
    {
        public override string[] NameCommand => List("/reload", "/перезагрузить");

        public override string NameClass => "Для перезагрузки всего(Configth, database(Json, xml), base people)";

        public override string Explanation => "/reload";

        public override Access Access => Access.SuperAdmin;

        public override string Move(Message message, VkApi bot)
        {
            bool success = true;

            try { ConfigMeneger.ReLoadConfigth(); }
            catch (Exception ex)
            {
                success = false;
                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload Configth");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload Configth", bot);
            }

            try { if (ConfigMeneger.Configth.NameSave.ToLower() == "json") { AdminsList.LoadJsonListAdmins(); } else if (ConfigMeneger.Configth.NameSave.ToLower() == "xml") { AdminsList.LoadXmlListAdmins(); } }
            catch (Exception ex)
            {
                success = false;
                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload databse");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload databse", bot);
            }

            try { RegisterList.LoadListUser(); }
            catch (Exception ex)
            {
                success = false;
                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload data user");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload data user", bot);
            }

            try { PeopleList.LoadPeople(); }
            catch (Exception ex)
            {
                success = false;
                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload data peoples");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload data peoples", bot);
            }

            try { QuestionsList.LoadQuestions(); }
            catch (Exception ex)
            {
                success = false;
                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload data questions");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} error reload data questions", bot);
            }

            if (success == true)
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Успешно", RandomId = new Random().Next() });

                return "Success";
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Ошибка", RandomId = new Random().Next() });

                return "Error";
            }
        }
    }
}
