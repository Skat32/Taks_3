using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Logic
{
    public static class Deserializer
    {
        public static object DeserializeObject(this object obj)
        {
            return JsonConvert.DeserializeObject(obj.ToString() ?? string.Empty,
                new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        public static string SerializeObject(this object obj, NullValueHandling nullValueHandling = NullValueHandling.Ignore, DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings{NullValueHandling = nullValueHandling, DefaultValueHandling = defaultValueHandling});
        }

        public static T DeserializeObject<T>(this object obj)
        {
            return JsonConvert.DeserializeObject<T>(obj.ToString() ?? string.Empty,
                new JsonSerializerSettings {Formatting = Formatting.Indented});
        }

        public static T DeserializeString<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str, new JsonSerializerSettings {Formatting = Formatting.Indented});
        }

        public static byte[] DeserializeObjectToBytes(this object obj)
        {
            if(obj == null)
                return null;
            
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            
            return ms.ToArray();
        }
    }
}