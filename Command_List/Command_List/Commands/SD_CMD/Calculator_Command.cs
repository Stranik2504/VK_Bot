using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;


namespace Command_List.Commands.SD_CMD
{
    class Calculator_Command : API_Commands
    {
        public override string[] NameCommand => List("/calculator", "/calculate", "/calc", "/калькулятор", "посчитай", "calculate", "calc");

        public override string NameClass => "Калькулятор";

        public override string Explanation => "/calculator {Пример для решения}";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            string converter = message.Text.Remove(0, (message.Text.Split(' ')[0] + " ").Length);
            string plusConverter = "";

            for (int i = 0; i < converter.Split('+').Length; i++)
            {
                if (i != converter.Split('+').Length - 1) { plusConverter += converter.Split('+')[i] + "%2B"; } else { plusConverter += converter.Split('+')[i]; }
            }

            converter = "";

            for (int i = 0; i < plusConverter.Split(' ').Length; i++)
            {
                converter += plusConverter.Split(' ')[i] + "";
            }

            string responseCalc = Get("http://api.mathjs.org/v4/?expr=" + converter, bot);

            bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = responseCalc, RandomId = new Random().Next() });

            return responseCalc;
        }
    }
}
