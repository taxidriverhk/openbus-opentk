using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OpenBus.Common;

namespace OpenBus.Config
{
    public static class XmlDeserializeHelper<T>
    {
        public static T DeserializeFromFile(string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new StreamReader(path));
            }
            catch (Exception ex)
            {
                Log.Write(LogLevel.Error, "Unable to deserialize the file {0}: {1}", path, ex.Message);
                return default(T);
            }
        }
    }
}
