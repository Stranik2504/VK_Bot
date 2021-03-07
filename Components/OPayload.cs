using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VK_Bot.Components
{
    public class OPayload : Output
    {
        private readonly string _payload;

        public string Payload { get => _payload; }

        public OPayload(string payload, string answer) : base(answer) => _payload = payload;
    }
}
