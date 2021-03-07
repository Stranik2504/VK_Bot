using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class People
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public int CorrectAnswer { get; set; }
        public int NumberQuestions { get; set; }
        public DateTime TimeEnd { get; set; }
    }
}
