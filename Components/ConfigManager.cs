using System;
using System.IO;

namespace VK_Bot.Components
{
    public static class ConfigManager
    {
        public static Config Configs { get; set; } = new Config();
        public static Action LoadConfigAction;
        public static Action SaveConfigAction;

        private const string NameFile = @"files/Config.conf";
        private readonly static Manager _manager = new Manager(NameFile);

        static ConfigManager()
        {
            _manager.LoadAction += LoadConfigAction;
            _manager.SaveAction += SaveConfigAction;
        }

        public static void LoadConfig()
        {
            if (!File.Exists(NameFile)) { _manager.Save(Configs); }
            Configs = _manager.Load<Config>();
            _manager.Invoke();
        }

        public static void SaveConfig()
        {
            _manager.Save(Configs);
            _manager.Invoke(false);
        }
    }
}
