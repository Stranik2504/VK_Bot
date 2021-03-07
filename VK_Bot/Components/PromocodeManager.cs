using System;
using System.IO;
using System.Collections.Generic;

namespace VK_Bot.Components
{
    public class PromocodeManager
    {
        public static List<(long userId, string domain, string promocode)> Promocodes { get; set; } = new List<(long userId, string domain, string promocode)>();
        public static Action LoadConfigAction;
        public static Action SaveConfigAction;

        private const string NameFile = "Add_promocode.sav";
        private readonly static Manager _manager = new Manager("Add_promocode.sav");

        static PromocodeManager()
        {
            _manager.LoadAction += LoadConfigAction;
            _manager.SaveAction += SaveConfigAction;
        }

        public static void LoadPromocode()
        {
            if (!File.Exists(NameFile)) { _manager.Save(Promocodes); }
            Promocodes = _manager.Load<List<(long userId, string domain, string promocode)>>();
            _manager.Invoke();
        }

        public static void SavePromocode()
        {
            _manager.Save(Promocodes);
            _manager.Invoke(false);
        }

        public static (long userId, string domain, string promocode) GetById(long userId)
        {
            foreach (var promocode in Promocodes) { if (promocode.userId == userId) { return promocode; } }

            return (-1, null, null);
        }
    }
}
