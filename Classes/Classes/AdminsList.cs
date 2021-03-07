using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json; 

namespace Classes
{
    public class AdminsList
    {
        public static List<Admin> Admins { get; private set; }
        private static Mutex mutexLoad = new Mutex();
        private static Mutex mutexSave = new Mutex();

        public static void LoadXmlListAdmins()
        {
            try
            {
                mutexLoad.WaitOne();

                FileStream stream = File.Open("Admins.xml", FileMode.Open);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Admin>));
                Admins = (List<Admin>)xmlSerializer.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[admins list ended load xml]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][admins list ended load xml]");

                mutexLoad.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(AdminsList load xml)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(AdminsList load xml)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(AdminsList load xml)]: {ex.Message}", null);
                try { mutexLoad.ReleaseMutex(); } catch { }
            }
        }

        public static void SaveXmlListAdmins()
        {
            try
            {
                mutexSave.WaitOne();

                FileStream stream = File.Open("Admins.xml", FileMode.Create);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Admin>));
                xmlSerializer.Serialize(stream, Admins);

                stream.Close();
                stream.Dispose();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[admins list ended save xml]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][admins list ended save xml]");

                mutexSave.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(AdminsList save xml)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(AdminsList save xml)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(AdminsList save xml)]: {ex.Message}", null);
                try { mutexSave.ReleaseMutex(); } catch { }
            }
        }

        public static void LoadJsonListAdmins()
        {
            try
            {
                mutexLoad.WaitOne();

                using (StreamReader reader = new StreamReader("Admins.json"))
                {
                    Admins = JsonConvert.DeserializeObject<List<Admin>>(reader.ReadLine());
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[admins list ended load json]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][admins list ended load json]");

                mutexLoad.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(AdminsList load json)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(AdminsList load json)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(AdminsList load json)]: {ex.Message}", null);
                try { mutexLoad.ReleaseMutex(); } catch { }
            }
        }

        public static void SaveJsonListAdmins()
        {
            try
            {
                mutexSave.WaitOne();

                using (StreamWriter writer = new StreamWriter("Admins.json"))
                {
                    string json = JsonConvert.SerializeObject(Admins);
                    writer.WriteLine(json);
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[admins list ended save json]");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][admins list ended save json]");

                mutexSave.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now}][exception(AdminsList save json)]: {ex.Message}");
                Console.ResetColor();
                Logger.Log($"[{DateTime.Now}][exception(AdminsList save json)]: {ex.Message}");
                ExceptionMove.Exception($"[{DateTime.Now}][exception(AdminsList save json)]: {ex.Message}", null);
                try { mutexSave.ReleaseMutex(); } catch { }
            }
        }

        public static void CreateAdmins()
        {
            Admins = new List<Admin>();
        }
    }
}
