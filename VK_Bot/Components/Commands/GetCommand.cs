using System;
using System.Collections.Generic;
using VK_Bot.Components.Commands.ACoins;
using VK_Bot.Components.Commands.Prize;
using VK_Bot.Components.Commands.Standard;

namespace VK_Bot.Components.Commands
{
    public static class GetCommand
    {
        private static Dictionary<Access, IReadOnlyList<Command>> _commands = new Dictionary<Access, IReadOnlyList<Command>>();

        public static IReadOnlyList<Command> GetCommands(Access access, bool regenarate = false)
        {
            if (!_commands.ContainsKey(access) || regenarate)
            {
                List<Command> commands = new List<Command>();

                commands.AddRange(GetModuleCommands(access, Module.Standard, GetCommands()));
                if (ConfigManager.Configs.IsActivateRaffle) { commands.AddCommands(GetModuleCommands(access, Module.Prize, GetCommands())); }
                if (ConfigManager.Configs.IsActivateACoins) { commands.AddCommands(GetModuleCommands(access, Module.ACoins, GetCommands())); }

                _commands.Add(access, commands.AsReadOnly());
            }

            return _commands[access];
        }

        private static IReadOnlyList<Command> GetModuleCommands(Access access, Module module, IReadOnlyList<Command> commands)
        {
            List<Command> models_commands = new List<Command>();

            foreach (var command in commands) { if (command.GetAccess().Contains(access) && command.GetModule() == module) { models_commands.Add(command); } }

            return models_commands;
        }

        private static IReadOnlyList<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();

            commands.Add(new Change_Config_Admin_Command());
            commands.Add(new Reload_Admin_Command());
            commands.Add(new Add_Prize_Admin_Command());
            commands.Add(new Add_User_Admin_Command());
            commands.Add(new Delete_Prize_Admin_Command());
            commands.Add(new Delete_User_Admin_Command());
            commands.Add(new Points_Admin_Command());
            commands.Add(new Post_Admin_Command());
            commands.Add(new Update_Prize_Admin_Command());
            commands.Add(new Update_User_Admin_Command());
            commands.Add(new GetCommands_Commad());
            commands.Add(new Info_Command());
            commands.Add(new Points_Command());
            commands.Add(new Roll_Command());
            commands.Add(new Start_Registration_Command());
            commands.Add(new ACoins_Command());
            commands.Add(new Promocode_Command());
            commands.Add(new Generate_Promocode_Command());
            commands.Add(new Generate_Promocode_Admin_Command());
            commands.Add(new ACoins_Info_Registration_Command());
            commands.Add(new Registration_In_Database_Command());
            commands.Add(new Registration_In_Database_Admin_Command());

            return commands.AsReadOnly();
        }
    }
}
