using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace VK_Bot.Components
{
    public class Config
    {
        public string Token { get; set; }
        public string UserToken { get; set; }
        public long Ts { get; set; }
        public long IdGroup { get; set; }
        public string NameGroup { get; set; }
        public long IdPost { get; set; }
        public string KeyWord { get; set; }
        public string AnswerWinTextComment { get; set; }
        public string AnswerLoseTextComment { get; set; }
        public string AnswerLoseTextDialogue { get; set; }
        public string ApiToken { get; set; }
        public string ApiTokenACoins { get; set; }
        public string BaseId { get; set; }
        public string BaseIdACoins { get; set; }
        public long StartPoints { get; set; }
        public long PostPoints { get; set; }
        // Content для Картинки
        public string TextPhoto { get; set; }
        public string NameFont { get; set; }
        public float FontSize { get; set; }
        public string TextColor { get; set; } 
        public int Radius { get; set; }
        // Проврека на тестера, либо на активацию розыгрыша, либо на активацию получения кол-ва ACoins и промокодов
        public bool IsActivateRaffle { get; set; }
        public bool IsLoadRaffleClient { get; set; }
        public bool IsActivateACoins { get; set; }
        public bool IsActivateACoinsButton { get; set; }
        public List<long> TestersIds { get; set; }
    }
}
