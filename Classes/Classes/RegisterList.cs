using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Threading;

namespace Classes
{
    public static class RegisterList
    {
        public static List<User> Users { get; private set; }
        private static Mutex mutexLoad = new Mutex();
        private static Mutex mutexSave = new Mutex();

        public static void LoadListUser()
        {
            try
            {
                mutexLoad.WaitOne();

                FileStream stream = File.Open("Users.user", FileMode.Open);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<User>));
                Users = (List<User>)xmlSerializer.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[users list ended load]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][users list ended load]");

                mutexLoad.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(RegisterList load)]: {ex.Message}");
                Console.WriteLine();
                Logger.Log($"[{DateTime.Now}][exception(RegisterList load)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(RegisterList load)]: {ex.Message}", null);
                try { mutexLoad.ReleaseMutex(); } catch { }
            }
        }

        public static void SaveListUser()
        {
            try
            {
                mutexSave.WaitOne();

                FileStream stream = File.Open("Users.user", FileMode.Create);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<User>));
                xmlSerializer.Serialize(stream, Users);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[users list ended save]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][users list ended save]");

                mutexSave.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(RegisterList save)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(RegisterList save)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(RegisterList save)]: {ex.Message}", null);
                try { mutexSave.ReleaseMutex(); } catch { }
            }
        }
    }
}
