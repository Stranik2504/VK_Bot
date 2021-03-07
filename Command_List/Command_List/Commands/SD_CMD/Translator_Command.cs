using System;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using Classes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace Command_List.Commands.SD_CMD
{
    class Translator_Command : API_Commands
    {
        public override string[] NameCommand => List("/translate", "/trans", "/tr", "/translator", "translate", "trans", "/переводчик", "/перев", "/пер", "перев", "переведи");

        public override string NameClass => "Переводчик";

        public override string Explanation => "/translate {Язык(Два символа языка на который хотите перевести(Например: ru, или en))} {Предложение(Слово, фраза)}";

        public override Access Access => Access.User;

        public override string Move(Message message, VkApi bot)
        {
            if (message.Text.Split(' ').Length >= 3)
            {
                string textToTranslate = message.Text.Remove(0, (message.Text.Split(' ')[0] + "  " + message.Text.Split(' ')[1]).Length);
                string response = Get("https://translate.yandex.net/api/v1.5/tr.json/translate?key=trnsl.1.1.20190720T075726Z.9ba09307c3eef338.059f3573c7633dcf326a3603aaf75ee1334564d4&text=" + textToTranslate + "&lang=" + message.Text.ToLower().Split(' ')[1], bot);

                var rObj = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response);

                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = rObj["text"][0], RandomId = new Random().Next() });

                return rObj["text"][0];
            }
            else
            {
                bot.Messages.Send(new MessagesSendParams() { UserId = message.PeerId.Value, Message = Explanation, RandomId = new Random().Next() });

                return Explanation;
            }
        }
    }
}
