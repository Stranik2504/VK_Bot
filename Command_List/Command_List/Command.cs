using System;
using VkNet;
using VkNet.Model;
using System.Data.SqlClient;
using Classes;

namespace Command_List
{
    public enum Access { Proger, SuperAdmin, Admin, User };

    public abstract class Command
    {
        public abstract string[] NameCommand { get; }

        public abstract string NameClass { get; }

        public abstract string Explanation { get; }

        public abstract Access Access { get; }

        public abstract string Move(Message message, VkApi bot);

        public virtual string Execute(Message message, VkApi bot)
        {
            try
            {
                if (message != null && bot != null && message.PeerId != null)
                {
                    string output = Move(message, bot);

                    Logger.Log($"[{DateTime.Now}][Answer to {message.PeerId.Value}][{NameClass}]: {output}");
                    return $"[{DateTime.Now}][Answer to {message.PeerId.Value}][{NameClass}]: {output}";
                }
                else
                {
                    Logger.Log($"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error");
                    return $"[{DateTime.Now}][Answer from {message.PeerId.Value}][{NameCommand[0]}]: error";
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}", bot);
                return $"[{DateTime.Now}][exception(command {NameClass})]: {ex.Message}";
            }
        }

        public bool Contains(string command)
        {
            foreach (var nameComm in NameCommand)
            {
                if (nameComm.ToLower().StartsWith(command.ToLower().Split(' ')[0]) == true && nameComm.ToLower() == command.ToLower().Split(' ')[0] == true)
                {
                    return true;
                }
            }

            return false;
        }

        public string[] List(params string[] obj)
        {
            return obj;
        }
    }
}
