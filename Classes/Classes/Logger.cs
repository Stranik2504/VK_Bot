using System;
using System.IO;
using System.Threading;

namespace Classes
{
    public static class Logger
    {
        public static bool IsWorking { get; set; }
        private static string path;
        private static bool activateMassage = false;
        private static Mutex mutex = new Mutex();

        public static void GetParams(string pathLog, bool isWorking)
        {
            path = pathLog;
            IsWorking = isWorking;
        }

        public static void Log(string logMessage)
        {
            if (IsWorking == true)
            {
                try
                {
                    mutex.WaitOne();

                    File.AppendAllText(path, logMessage + "\n");

                    mutex.ReleaseMutex();
                }
                catch (Exception ex) { if (activateMassage == false) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"[{DateTime.Now}][exception logging]: {ex.Message}"); activateMassage = true; Console.ResetColor(); } try { mutex.ReleaseMutex(); } catch { } }
            }
        }

        public static void Clear()
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(path);
                streamWriter.Close();
                streamWriter.Dispose();
                Console.WriteLine("[log cleared]");
            }
            catch (Exception ex) { if (activateMassage == false) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"[{DateTime.Now}][exception logging]: {ex.Message}"); activateMassage = true; Console.ResetColor(); } }
        }
    }
}
