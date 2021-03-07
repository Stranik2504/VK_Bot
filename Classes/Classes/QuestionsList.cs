using System;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using System.Collections.Generic;

namespace Classes
{
    public class QuestionsList
    {
        public static List<Questions> Questions { get; private set; }
        private static Mutex mutexLoad = new Mutex();
        private static Mutex mutexSave = new Mutex();

        public static void LoadQuestions()
        {
            try
            {
                mutexLoad.WaitOne();

                FileStream stream = File.Open("Questions.que", FileMode.Open);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Questions>));
                Questions = (List<Questions>)xmlSerializer.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[question ended reload]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][question ended reload]");

                mutexLoad.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(questionsList reload)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(questionsList reload)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(questionsList reload)]: {ex.Message}", null);
                try { mutexLoad.ReleaseMutex(); } catch { }
            }
        }

        public static void SaveQuestions()
        {
            try
            {
                mutexSave.WaitOne();

                FileStream stream = File.Open("Questions.que", FileMode.Create);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Questions>));
                xmlSerializer.Serialize(stream, Questions);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[question ended save]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][question ended save]");

                mutexSave.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(questionsList save)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(questionsList save)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(questionsList save)]: {ex.Message}", null);
                try { mutexSave.ReleaseMutex(); } catch { }
            }
        }
    }
}
