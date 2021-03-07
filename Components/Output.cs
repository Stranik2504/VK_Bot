using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VK_Bot.Components
{
    public class Output
    {
        private readonly string _answer;

        public string Answer { get => _answer; }

        public Output(string answer) => _answer = answer;
    }
}