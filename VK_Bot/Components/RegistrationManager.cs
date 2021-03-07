using System;
using System.IO;
using System.Collections.Generic;

namespace VK_Bot.Components
{
    public class RegistrationManager
    {
        public static List<(long userId, string domain, string name)> Users { get; set; } = new List<(long userId, string domain, string name)>();
        public static Action LoadConfigAction;
        public static Action SaveConfigAction;

        private const string NameFile = "Users.reg";
        private readonly static Manager _manager = new Manager(NameFile);

        static RegistrationManager()
        {
            _manager.LoadAction += LoadConfigAction;
            _manager.SaveAction += SaveConfigAction;
        }

        public static void LoadUsers()
        {
            if (!File.Exists(NameFile)) { _manager.Save(Users); }
            Users = _manager.Load<List<(long userId, string domain, string name)>>();
            _manager.Invoke();
        }

        public static void SaveUsers()
        {
            _manager.Save(Users);
            _manager.Invoke(false);
        }
    }
}
