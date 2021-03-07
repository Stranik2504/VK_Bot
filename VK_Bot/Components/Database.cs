using System;
using System.Collections.Generic;
using System.Linq;
using AirtableApiClient;
using System.IO;

using static System.Diagnostics.Debug;
using Newtonsoft.Json.Linq;

namespace VK_Bot.Components
{
    public enum Place { Users = 1, Prizes = 2, Lists = 3, Promocode = 4, Admins = 6, Wallet = 7, ClubCard = 8 }

    public static class Database
    {
        public static Dictionary<Place, (string Search, string NameTable)> Default { get; private set; } = new Dictionary<Place, (string Search, string NameTable)>();

        private static AirtableBase _client;
        private static AirtableBase _clientACoins;

        static Database()
        {
            Default.Add(Place.Users, ("UserId", "Users"));
            Default.Add(Place.Prizes, ("Id", "Prizes"));
            Default.Add(Place.Lists, ("Name", "Списки"));
            Default.Add(Place.Promocode, ("Название", "Промокоды"));
            Default.Add(Place.Admins, ("UserId", "Admins"));
            Default.Add(Place.Wallet, ("-Id", "Кошелек"));
            Default.Add(Place.ClubCard, ("VK", "Клубная карта"));
        }

        public static void Start(bool isStartClientPrize = true, bool isClientACoins = true)
        {
            if (isStartClientPrize) { try { _client = new AirtableBase(ConfigManager.Configs.ApiToken, ConfigManager.Configs.BaseId); } catch (Exception ex) { $"[Database][Start] : {ex.Message}".Log(); } }
            if (isClientACoins) { try { _clientACoins = new AirtableBase(ConfigManager.Configs.ApiTokenACoins, ConfigManager.Configs.BaseIdACoins); } catch (Exception ex) { $"[Database][Start] : {ex.Message}".Log(); } }
        }

        public static void End()
        {
            try { _client.Dispose(); _clientACoins.Dispose(); } catch (Exception ex) { $"[Database][End]: {ex.Message}".Log(); }
        }

        public static bool IsInDatabase(Place place, string userId)
        {
            try
            {
                if (place == Place.Users || place == Place.ClubCard)
                {
                    var res = GetValueData<string>(place, userId).Id;

                    return res != null && res != "" && res != " ";
                }

                return false;
            }
            catch (Exception ex) { $"[Database][IsInDatabase]: {ex.Message}".Log(); return false; }
        }

        public static Access GetAccessInDatabase(string userId)
        {
            try
            {
                var res = GetCells(Place.Admins).Where(x => x.Fields.ContainsKey("UserId") && x.Fields["UserId"].ToString() == userId);
                if (res.Count() > 0) { return (Access)long.Parse(res.First().Fields["Level"].ToString()); } else { if (IsInDatabase(Place.ClubCard, userId)) { return Access.User; } else { return Access.NotIndexed; } }
            }
            catch { return Access.NotIndexed; }
        }

        public static bool AddInDatabase(Access access, long userId) => AddInDatabase(access, GetNameUser(userId), userId, ConfigManager.Configs.StartPoints);

        public static bool AddInDatabase(Access access, long userId, long points) => AddInDatabase(access, GetNameUser(userId), userId, points);

        public static bool AddInDatabase(Access access, string name, long userId, long points)
        {
            try
            {
                return CreateElement(GetBase(Place.Users), Default[Place.Users].NameTable, new Dictionary<string, object>() { {"UserId", userId }, { "Access", (int)access }, { "Username", name }, { "Points", points } }).Success;
            }
            catch (Exception ex) { $"[Database][AddInDatabase(with params)]: {ex.Message}".Log(); return false; }
        }

        public static bool CreateNewPromocode(string name, long discount, string description, long count = 100) => CreateNewPromocode(name, discount, description, DateTime.Now, new DateTime(2021, 12, 31), count);

        public static bool CreateNewPromocode(string name, long discount, string description, DateTime startTime, DateTime endTime, long count = 100)
        {
            try
            {
                return CreateElement(GetBase(Place.Promocode), Default[Place.Promocode].NameTable, new Dictionary<string, object>() { { "Название", name }, { "Скидка", discount }, { "Описание", description }, { "Количество", count }, { "Начало", startTime.ToShortDateString().ToAirtableDate() }, { "Окончание", endTime.ToShortDateString().ToAirtableDate() } }).Success;
            }
            catch (Exception ex) { $"[Database][CreateNewPromocode(with date)]: {ex.Message}".Log(); return false; }
        }

        public static bool AddLinkToCard(string numberCard, string domain)
        {
            try
            {
                var res = GetValueData<string>(Place.ClubCard, numberCard, searchByField: "Номер");
                return UpdateElement(GetBase(Place.ClubCard), Default[Place.ClubCard].NameTable, res.Id, new Dictionary<string, object>() { { "VK", "https://vk.com/" + domain } });
            }
            catch (Exception ex) { $"[Database][AddLinkToCard]: {ex.Message}".Log(); return false; }
        }

        public static bool AddClubCard(string fio, string domain)
        {
            try
            {
                var walletId = new List<string>() { CreateElement(GetBase(Place.Wallet), Default[Place.Wallet].NameTable, new Dictionary<string, object>() { { "Название", fio } }).Id };
                var idUser = new List<string>() { GetValueData<string>(Place.Lists, fio).Id };
                return CreateElement(GetBase(Place.ClubCard), Default[Place.ClubCard].NameTable, new Dictionary<string, object>() { { "VK", "https://vk.com/" + domain }, { "Кошелек", walletId }, { "Номер", DateTime.Now.ToNumber() }, { "Владелец", idUser } }).Success;
            }
            catch (Exception ex) { $"[Database][AddClubCard]: {ex.Message}".Log(); return false; }
        }

        public static bool AddUser(string fio, string domain)
        {
            try
            {
                var walletId = new List<string>() { CreateElement(GetBase(Place.Wallet), Default[Place.Wallet].NameTable, new Dictionary<string, object>() { { "Название", fio } }).Id };
                var idUser = new List<string>() { CreateElement(GetBase(Place.Lists), Default[Place.Lists].NameTable, new Dictionary<string, object>() { { "Name", fio } }).Id };
                return CreateElement(GetBase(Place.ClubCard), Default[Place.ClubCard].NameTable, new Dictionary<string, object>() { { "VK", "https://vk.com/" + domain }, { "Кошелек", walletId }, { "Номер", DateTime.Now.ToNumber() }, { "Владелец", idUser } }).Success;
            }
            catch (Exception ex) { $"[Database][AddClubCard]: {ex.Message}".Log(); return false; }
        }

        public static bool AddPromocodeToUser(string userId, params string[] promocodes)
        {
            try
            {
                JArray arr = GetValueData<JArray>(Place.ClubCard, userId, "Промокоды").Field ?? new JArray();
                foreach (var promocode in promocodes) { arr.AddFirst(GetValueData<string>(Place.Promocode, promocode, "-Id")); }

                Dictionary<string, object> Fields = new Dictionary<string, object>();
                Fields.Add("Промокоды", arr);

                var res = GetBase(Place.ClubCard).UpdateRecord(Default[Place.ClubCard].NameTable, new Fields() { FieldsCollection = Fields }, GetValueData<string>(Place.ClubCard, userId).Id).Result.Success;
                return res;
            }
            catch (Exception ex) { $"[Database][AddPromocodeToUser]: {ex.Message}".Log(); return false; }
        }

        public static bool DeleteCellInDatabase(Place place, string id)
        {
            try
            {
                switch (place)
                {
                    case Place.Prizes:
                    case Place.Users:
                        string recId = GetValueData<string>(place, id.ToString()).Id;
                        if (recId != null && recId != "") { return DeleteElement(GetBase(place), Default[place].NameTable, recId); } else { return false; }

                    case Place.Lists:
                        return false;
                }

                return false;
            }
            catch (Exception ex) { $"[Database][DeleteCellInDatabase]: {ex.Message}".Log(); return false; }
        }

        public static bool AddPrizeInDatabase(string Text, string Filename)
        {
            try
            {
                return CreateElement(GetBase(Place.Prizes), Default[Place.Prizes].NameTable, new Dictionary<string, object>() { { "Text", Text }, { "Count", 0 }, { "Photo", @"Photo\" + Filename } }).Success;
            }
            catch (Exception ex) { $"[Database][AddPrizeInDatabase]: {ex.Message}".Log(); return false; }
        }

        public static IEnumerable<AirtableRecord> GetCells(Place place)
        {
            AirtableListRecordsResponse list = null;
            AirtableBase client = GetBase(place);

            if (!BlockInTryGetCells(() => { list = client.ListRecords(Default[place].NameTable).Result; })) { yield break; }
            do
            {
                foreach (var record in list.Records) { yield return record; }
                if (!BlockInTryGetCells(() => { list = client.ListRecords(Default[place].NameTable, offset: list.Offset).Result; })) { yield break; }
            }
            while (list != null && list.Offset != null);

            yield break;
        }

        public static (T Field, string Id) GetValueData<T>(Place place, string field, string nameSearchField = null, string searchByField = null)
        {
            try
            {
                AirtableBase client = GetBase(place);
                string tableName = Default[place].NameTable;

                if (searchByField == null || searchByField == "") { searchByField = Default[place].Search; }
                if (nameSearchField == null || nameSearchField == "") { nameSearchField = Default[place].Search; }

                if (searchByField == "VK")
                {
                    (T Field, string Id) results = GetValueData<T>(client, tableName, searchByField, "http://m.vk.com/" + field, nameSearchField);
                    if (results.Id != null && results.Id != "") { return results; }

                    results = GetValueData<T>(client, tableName, searchByField, "https://m.vk.com/" + field, nameSearchField);
                    if (results.Id != null && results.Id != "") { return results; }

                    results = GetValueData<T>(client, tableName, searchByField, "http://vk.com/" + field, nameSearchField);
                    if (results.Id != null && results.Id != "") { return results; }

                    results = GetValueData<T>(client, tableName, searchByField, "https://vk.com/" + field, nameSearchField);
                    if (results.Id != null && results.Id != "") { return results; } else { return (default(T), null); }
                }

                if (client != null) { return GetValueData<T>(client, tableName, searchByField, field, nameSearchField); }

                return (default(T), null);
            }
            catch (Exception ex) { $"[Database][GetValueData]: {ex.Message}".Log(); return (default(T), null); }
        }

        private static (T Field, string Id) GetValueData<T>(AirtableBase client, string nameTable, string searchByField, string field, string nameSearchField)
        {
            try
            {
                AirtableRecord record = null;

                if (searchByField != "-Id") { var results = client.ListRecords(nameTable, filterByFormula: "FIND(\"" + field + "\", {" + searchByField + "})").Result; record = results.Success && results.Records.Count() > 0 ? results.Records.Last() : null; }
                else { var results = client.RetrieveRecord(nameTable, field).Result; record = results.Success ? results.Record : null; }

                return ((T)(record != null ? (record.Fields.ContainsKey(nameSearchField) ? record.Fields[nameSearchField] : default(T)) : default(T)), record != null ? record.Id : null);
            }
            catch { throw; }
        }

        public static bool SetValueData<T>(Place place, string field, string nameField, T param, string searchByField = null)
        {
            try
            {
                var res = GetBase(place).UpdateRecord(Default[place].NameTable, new Fields() { FieldsCollection = (nameField, param).ToDictionary() }, GetValueData<string>(place, field, searchByField: searchByField).Id).Result;
                return res.Success;
            }
            catch (Exception ex) { $"[Database][SetValueData]: {ex.Message}".Log(); return false; }
        }

        public static (string Text, bool IsWin, string[] Photos) GetRandomPrize()
        {
            try
            {
                int[] nums = new int[9];
                var res = GetBase(Place.Prizes).ListRecords(Default[Place.Prizes].NameTable).Result;
                var rand = Random();

                if (rand.number1 == 1) { nums[0] = res.Records.Count(); } else { nums[0] = rand.number1 - 1; }
                if (rand.number2 == 1) { nums[1] = res.Records.Count(); } else { nums[1] = rand.number2 - 1; }
                if (rand.number3 == 1) { nums[2] = res.Records.Count(); } else { nums[2] = rand.number3 - 1; }

                nums[3] = rand.number1;
                nums[4] = rand.number2;
                nums[5] = rand.number3;

                if (rand.number1 == res.Records.Count()) { nums[6] = 1; } else { nums[6] = rand.number1 + 1; }
                if (rand.number2 == res.Records.Count()) { nums[7] = 1; } else { nums[7] = rand.number2 + 1; }
                if (rand.number3 == res.Records.Count()) { nums[8] = 1; } else { nums[8] = rand.number3 + 1; }

                if (res.Success == true)
                {
                    string[] prizes = new string[9];
                    string text = "";

                    for (int i = 0; i < prizes.Length; i++)
                    {
                        prizes[i] = GetCells(Place.Prizes).ToList()[nums[i]].Fields["Photo"].ToString();
                    }

                    if (rand.number1 == rand.number2 && rand.number1 == rand.number3)
                    {
                        var prize = GetCells(Place.Prizes).ToList()[nums[3]];
                        AddCountPrize(long.Parse(prize.Fields["Id"].ToString()));
                        text = prize.Fields["Text"].ToString();
                    }

                    return (text, rand.number1 == rand.number2 && rand.number1 == rand.number3, prizes);
                }

                return ("Error", false, null);
            }
            catch (Exception ex) { $"[Database][GetRandomPrize]: {ex.Message}".Log(); return ("Error", false, null); }
        }

        public static int CountValues(Place place)
        {
            try
            {
                int count = 0;

                var res = GetBase(place).ListRecords(Default[place].NameTable).Result;

                while (res != null && res.Offset != null)
                {
                    count += res.Records.Count();

                    res = GetBase(place).ListRecords(Default[place].NameTable, offset: res.Offset).Result;
                }

                return count;
            }
            catch (Exception ex) { $"[Database][CountValues]: {ex.Message}".Log(); return 0; }
        }

        public static void Log(long FromId, long ToId, long Points, string Move)
        {
            try { File.AppendAllText(@"Data.log", "[" + DateTime.Now.ToString() + "]" + $"[FromId]: {FromId}, [ToId]: {ToId}, [Operation]: {Move}, [Points]: {Points}" + " \n"); } catch (Exception ex) { $"[Database][Log]: {ex.Message}".Log(); }
        }

        private static bool UpdateElement(AirtableBase client, string nameTable, string id, Dictionary<string, object> fields)
        {
            try
            {
                var res = client.UpdateRecord(nameTable, new Fields() { FieldsCollection = fields }, id).Result;
                if (!res.Success && res.AirtableApiError is AirtableInvalidRequestException) { $"[Database][UpdateElement]: {(res.AirtableApiError as AirtableInvalidRequestException).DetailedErrorMessage}".Log(); }
                else { $"[Database][UpdateElement]: {res.AirtableApiError.ErrorMessage}".Log(); }
                return res.Success;
            }
            catch { throw; }
        }

        private static (bool Success, string Id) CreateElement(AirtableBase client, string nameTable, Dictionary<string, object> fields)
        {
            try
            {
                var res = client.CreateRecord(nameTable, new Fields() { FieldsCollection = fields }).Result;
                if (!res.Success && res.AirtableApiError is AirtableInvalidRequestException) { $"[Database][CreateElement]: {(res.AirtableApiError as AirtableInvalidRequestException).DetailedErrorMessage}".Log(); }
                else { $"[Database][CreateElement]: {res.AirtableApiError.ErrorMessage}".Log(); }
                return (res.Success, res.Record.Id);
            }
            catch { throw; }
        }

        private static bool DeleteElement(AirtableBase client, string nameTable, string id)
        {
            try
            {
                var res = client.DeleteRecord(nameTable, id).Result;
                if (!res.Success && res.AirtableApiError is AirtableInvalidRequestException) { $"[Database][DeleteElement]: {(res.AirtableApiError as AirtableInvalidRequestException).DetailedErrorMessage}".Log(); }
                else { $"[Database][DeleteElement]: {res.AirtableApiError.ErrorMessage}".Log(); }
                return res.Success;
            }
            catch { throw; }
        }

        private static AirtableBase GetBase(Place place)
        {
            switch (place)
            {
                case Place.Prizes:
                case Place.Users:
                case Place.Admins:
                default:
                    return _client;
                case Place.ClubCard:
                case Place.Wallet:
                case Place.Promocode:
                case Place.Lists:
                    return _clientACoins;
            }
        }

        private static string GetNameUser(long userId)
        {
            try { return Bot.GetNameUser(userId); } catch (Exception ex) { $"[Database][GetNameUser]: {ex.Message}".Log(); return ""; }
        }

        private static (int number1, int number2, int number3) Random()
        {
            Random _random = new Random();
            int num1 = _random.Next(CountValues(Place.Prizes)) + 1;
            int num2 = _random.Next(CountValues(Place.Prizes)) + 1;
            int num3 = _random.Next(CountValues(Place.Prizes)) + 1;
            return (num1, num2, num3);
        }

        private static bool AddCountPrize(long id)
        {
            try
            {
                return SetValueData(Place.Prizes, id.ToString(), "Count", GetValueData<long>(Place.Prizes, id.ToString(), "Count").Field + 1);
            }
            catch (Exception ex) { $"[Database][AddCountPrize]: {ex.Message}".Log(); return false; }
        }

        private static bool BlockInTryGetCells(Action action)
        {
            try { action?.Invoke(); } catch (Exception ex) { $"[Database][GetCells]: {ex.Message}".Log(); return false; }

            return true;
        }
    }
}
