using System;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace Classes
{
    public static class ConfigMeneger
    {
        public static Configth Configth { get; private set; }
        private static Mutex mutexReLoad = new Mutex();
        private static Mutex mutexSave = new Mutex();

        public static void ReLoadConfigth()
        {
            try
            {
                mutexReLoad.WaitOne();

                FileStream stream = File.Open("Configth.conf", FileMode.Open);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configth));
                Configth = (Configth)xmlSerializer.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[configth ended reload]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][configth ended reload]");

                mutexReLoad.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(configMeneger reload)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(configMeneger reload)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(configMeneger reload)]: {ex.Message}", null);
                try { mutexReLoad.ReleaseMutex(); } catch { }
            }
        }

        public static void SaveConfigth()
        {
            try
            {
                mutexSave.WaitOne();

                FileStream stream = File.Open("Configth.conf", FileMode.Create);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configth));
                xmlSerializer.Serialize(stream, Configth);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[configth ended save]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][configth ended save]");

                mutexSave.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(configMeneger save)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(configMeneger save)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(configMeneger save)]: {ex.Message}", null);
                try { mutexSave.ReleaseMutex(); } catch { }
            }
        }
    }
}
