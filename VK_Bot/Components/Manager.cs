using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace VK_Bot.Components
{
    public class Manager
    {
        public Action LoadAction;
        public Action SaveAction;

        private readonly Mutex _saveLoad = new Mutex();
        private readonly string _filename;

        public Manager(string filename) => _filename = filename; 

        public T Load<T>()
        {
            try
            {
                _saveLoad.WaitOne();

                var stream = File.Open(_filename, FileMode.Open);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                T obj = (T)xmlSerializer.Deserialize(stream);

                stream.Dispose();

                _saveLoad.ReleaseMutex();

                return obj;
            }
            catch (Exception ex) { _saveLoad.Close(); $"[Manager][Load]: {ex.Message}".Log(); throw; }
        }

        public void Save<T>(T saveParams)
        {
            try
            {
                _saveLoad.WaitOne();

                var stream = File.Open(_filename, FileMode.Create);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, saveParams);

                stream.Dispose();

                _saveLoad.ReleaseMutex();
            }
            catch (Exception ex) { _saveLoad.Close(); $"[Manager][Save]: {ex.Message}".Log(); throw; }
        }

        public void Invoke(bool isLoad = true)
        {
            if (isLoad) { try { LoadAction?.Invoke(); } catch { } } else { try { SaveAction?.Invoke(); } catch { } }
        }
    }
}
