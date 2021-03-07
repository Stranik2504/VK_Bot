using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using System.Net.Http;
using System.Net;
using AngleSharp.Html.Dom;

namespace Command_List.Commands
{
    public class Add_Admin_Command : Admin_Command
    {
        public override string[] NameCommand => List("/add_adm", "/new", "/добавить_адм");

        public override string NameClass => "Команда для добавление админов";

        public override string Explanation => "/add_adm {User id} {Доступ}";

        public override Access Access => Access.Admin;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length == 3)
            {
                if (message.Text.Split(' ')[1] != null && message.Text.Split(' ')[1] != "" && message.Text.Split(' ')[1] != " ")
                {
                    int.TryParse(message.Text.Split(' ')[1], out int UserId);
                    int.TryParse(message.Text.Split(' ')[2], out int AccessLevel);

                    if (AccessLevel < numberAccess) { AccessLevel = numberAccess; }

                    if (AccessLevel < Convert.ToInt32(Access.User) && AccessLevel >= 0)
                    {
                        string mess = AddToBase(UserId, AccessLevel, bot);

                        bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = mess, RandomId = new Random().Next() });

                        return $"The message was sent to user with id: {UserId}; message = {mess}";
                    }
                    else
                    {
                        bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = "Такого доступа не существует", RandomId = new Random().Next() });

                        return "There is no such access";
                    }
                }
                else
                {
                    return $"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error";
                }
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = Explanation, RandomId = new Random().Next() });

                return Explanation;
            }
        } 

        private string AddToBase(long UserId, int AccessLevel, VkApi bot)
        {
            switch (ConfigMeneger.Configth.NameSave.ToLower())
            {
                case "database":
                    return Database(UserId, AccessLevel, bot);
                case "xml":
                    return XmlAndJson(UserId, AccessLevel, false);
                case "json":
                    return XmlAndJson(UserId, AccessLevel, true);
                default:
                    return $"Ошибка: невозможно добавить юзера {UserId} в базу админов";
            }
        }

        private string Database(long UserId, int AccessLevel, VkApi bot)
        {
            using (SqlConnection connection = new SqlConnection(stringBuilder.ConnectionString))
            {
                connection.Open();

                string Name = "";
                while (ParseName(UserId).Result == null) { }
                if (ConfigMeneger.Configth.ParseNameAdmin == true) { Name = ParseName(UserId).Result; }

                using (SqlCommand command = new SqlCommand($"INSERT INTO Admins (Name, UserId, Access) VALUES ('{Name}', {UserId}, {AccessLevel})", connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in AddToBase");
                        ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message} in AddToBase", bot);
                        return $"Ошибка: невозможно добавить юзера {UserId} в базу админов";
                    }
                }

                return $"юзер {UserId} добавлен в базу админов с доступом: {AccessLevel}";
            }
        }

        private string XmlAndJson(long UserId, int AccessLevel, bool IsJson)
        {
            string Name = "";
            while (ParseName(UserId).Result == null) { }
            if (ConfigMeneger.Configth.ParseNameAdmin == true) { Name = ParseName(UserId).Result; }

            AdminsList.Admins.Add(new Admin() { UserId = UserId, Name = Name, Access = AccessLevel });

            if (IsJson == true) { AdminsList.SaveJsonListAdmins(); } else { AdminsList.SaveXmlListAdmins(); }

            return $"Юзер {UserId} добавлен в базу админов с доступом: {AccessLevel}";
        }

        private async Task<string> ParseName(long UserId)
        {
            var source = await GetSource(UserId);
            var domParser = new HtmlParser();

            var document = domParser.ParseDocument(source);

            var results = Parse(document);

            return results[0];
        }

        private async Task<string> GetSource(long UserId)
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://vk.com/id" + UserId);
            string source = null;

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                source = await response.Content.ReadAsStringAsync();
            }

            return source;
        }

        private string[] Parse(IHtmlDocument htmlDocument)
        {
            List<string> items = new List<string>();
            var lists = htmlDocument.QuerySelectorAll("title");
            foreach (var item in htmlDocument.QuerySelectorAll("title"))
            {
                items.Add(item.TextContent);
            }
            return items.ToArray();
        }
    }
}
