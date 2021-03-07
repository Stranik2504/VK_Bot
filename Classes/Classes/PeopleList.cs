using System;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using System.Collections.Generic;

namespace Classes
{
    public class PeopleList
    {
        public static List<People> Peoples { get; private set; }
        private static Mutex mutexLoad = new Mutex();
        private static Mutex mutexSave = new Mutex();

        public static void LoadPeople()
        {
            try
            {
                mutexLoad.WaitOne();

                FileStream stream = File.Open("Peoples.peop", FileMode.Open);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<People>));
                Peoples = (List<People>)xmlSerializer.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[peoples ended reload]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][peoples ended reload]");

                mutexLoad.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(peopleList reload)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(peopleList reload)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(peopleList reload)]: {ex.Message}", null);
                try { mutexLoad.ReleaseMutex(); } catch { }
            }
        }

        public static void SavePeople()
        {
            try
            {
                mutexSave.WaitOne();

                FileStream stream = File.Open("Peoples.peop", FileMode.Create);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<People>));
                xmlSerializer.Serialize(stream, Peoples);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[peoples ended save]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][peoples ended save]");

                mutexSave.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(peoplesList save)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(peoplesList save)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(peoplesList save)]: {ex.Message}", null);
                try { mutexSave.ReleaseMutex(); } catch { }
            }
        }
    }
}
