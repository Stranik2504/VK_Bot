using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class Configth
    {
        public string Token { get; set; }
        public ulong IdGroup { get; set; }
        public int Wait { get; set; }
        public int IdUserId { get; set; }
        public int IdPoints { get; set; }
        public int IdPromocode { get; set; }
        public int IdDiscount { get; set; }
        public string NamePoints { get; set; }
        public string NameSave { get; set; }
        public string Ts { get; set; }
        public string Account { get; set; }
        public string Login { get; set; }
        public string Hash { get; set; }
        public string Message { get; set; }
        public string GetAnswer { get; set; }
        public bool ActivateMessageSend { get; set; }
        public bool IsWorkingLoging { get; set; }
        public bool IsWorkingSeeMessage { get; set; }
        public bool ParseNameAdmin { get; set; }
        public bool IsActivateExceptionMove { get; set; }
        public bool IsRegistretingQuest { get; set; }
        public bool IsWorkingQuest { get; set; }
        public bool IsStartOn { get; set; }
        public bool IsStopOn { get; set; }
        public bool IsActivateSayModule { get; set; }
        public bool IsActivateCommentsModule { get; set; }
        public bool IsActivateEhoBot { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime TimeQuest { get; set; }
    }
}
