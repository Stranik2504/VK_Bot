using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Attachments;

namespace VK_Bot.Components
{
    public enum Visibility { Visible = 0, Hidden = 1 }
    public enum Module { Standard = 0, Prize = 1, ACoins = 2 }
    public enum FindModule { All = -1, Standard = Module.Standard, Prize = Module.Prize, ACoins = Module.ACoins }
    public enum Access { NotIndexed = 0, User = 1, Admin = 2, Programmer = 3, Bot = 4 }
    public enum Additions { UserId = 0, Domain = 1, Attachments = 2, ReplyUserId = 3 }

    public abstract class Command
    {
        private readonly List<string> _names = new List<string>();

        public abstract Visibility GetVisibility();
        public abstract Module GetModule();
        public abstract Access[] GetAccess();
        public string[] GetNames() => _names.ToArray();
        public void RemoveNameAt(int index) => _names.RemoveAt(index);
        public abstract string Description();
        public abstract Output Move(string message, Dictionary<Additions, string> additions);

        protected void SetNames(params string[] nums) => _names.AddRange(nums);
        protected Access[] SetAccess(params Access[] access) => access;
    }
}
