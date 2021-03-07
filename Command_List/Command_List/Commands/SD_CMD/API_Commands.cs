using System;
using VkNet;
using VkNet.Model;
using System.Data.SqlClient;
using Classes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.IO;


namespace Command_List.Commands.SD_CMD
{
    public abstract class API_Commands : Command
    {
        public static string Get(string uri, VkApi bot)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                ExceptionMove.Exception($"[{DateTime.Now}][exception(command Get)]: {ex.Message}", bot);
                return "";
            }

        }
    }
}
