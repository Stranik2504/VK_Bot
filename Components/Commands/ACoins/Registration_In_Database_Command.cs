using System;
using System.Collections.Generic;

namespace VK_Bot.Components.Commands.ACoins
{
    public class Registration_In_Database_Command : Command
    {
        public Registration_In_Database_Command() => SetNames("/registration_in_database");

        public override Visibility GetVisibility() => Visibility.Hidden;

        public override Module GetModule() => Module.ACoins;

        public override Access[] GetAccess() => SetAccess(Access.NotIndexed, Access.Admin, Access.Programmer, Access.Bot);

        public override string Description() => "Команда для регистрации";

        public override Output Move(string message, Dictionary<Additions, string> additions)
        {
            try
            {
                if (!RegistrationManager.Users.ContainsUserId(additions[Additions.UserId].ToLong()))
                {
                    string fioInfo = CheckFIO(message.Remove(0, (message.Split(' ')[0] + " ").Length));

                    if (message.Split(' ').Length >= 2 && fioInfo == null)
                    {
                        string fio = message.Remove(0, (message.Split(' ')[0] + " ").Length);

                        bool isTryOk = Bot.TrySendTriggerAdmins(AdminMessage(additions[Additions.UserId], additions[Additions.Domain], fio).Answer, (AdminMessage(additions[Additions.UserId], additions[Additions.Domain], fio) as OPayload).Payload);

                        RegistrationManager.Users.Add((additions[Additions.UserId].ToLong(), additions[Additions.Domain], fio));
                        RegistrationManager.SaveUsers();

                        if (!isTryOk) { $"[Registration_In_Database_Command][TrySendTriggerAdmins]: сообщение не отправленно".Log(); }

                        return "Заявка отправлена. Ожидайте подтверждение администрации в течение двух дней".ToOutput();
                    }
                    else { return (fioInfo == null ? "Надо написать ФИО полностью(3 слова)" : fioInfo).ToOutput(); }
                }
                else { return "Запрос уже отправлен".ToOutput(); }
            }
            catch (Exception ex) { $"[Registration_In_Database_Command]: {ex.Message}".Log(); }

            return "Ошибка".ToOutput();
        }

        public Output AdminMessage(string userId, string domain, string fio)
        {
            var idUser = Database.GetValueData<string>(Place.Lists, fio).Id;
            string output = "";
            if (idUser != null) { var wallet = Database.GetValueData<string>(Place.ClubCard, idUser, nameSearchField: "Номер", searchByField: "Владелец"); if (wallet.Id != null) { output += $", в таблице \"Клубная карта\": найден(Номер карты: {wallet.Field})"; } else { output += ", в таблице \"Клубная карта\": не найден"; } }
            return $"Юзер: {fio}({"https://vk.com/" + domain}) [В таблице \"Списки\": {(idUser != null ? "найден" : "не найден") + output}], хочет зарегистрироваться(Если вы поддверждаете регистрацию, то отправте в ответ на это сообщение: \"Да\", если её нет в таблице спииски. Если у юзера уже есть клубная крата, то отправте \"Да card {"{номер карты}"}\". Если есть запись в таблице \"Списки\", то отправте \"Да {"{ФИО из таблицы}"}\". Иначе отправьте \"Нет\")".ToOutput("/add_user " + userId + " " + domain.Replace(' ', '\0') + " {{" + fio + "}}");
        }

        private string CheckFIO(string fio)
        {
            if (fio.Length == 3)
            {
                if (fio.IsLetters())
                {
                    return null;
                }
                else { return "ФИО не может содержать числа"; }
            }
            else { return "Надо написать ФИО полностью(3 слова)"; }
        }
    }
}
