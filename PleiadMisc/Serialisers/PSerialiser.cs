using PleiadExtensions.Files;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace PleiadMisc.Serialisers
{
    public class PSerialiser
    {
        BinaryFormatter _formatter = new();


        public Task SerialiseAsync(object data, FileContract file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file.FileName));


            return Task.Run(() =>
            {
                using var ms = new MemoryStream();
                _formatter.Serialize(ms, data);
                using FileStream fs = file.Open(FileAccess.Write);
                var arr = ms.ToArray();
                fs.Write(arr);
            });
        }
        public Task<object> DeserialiseAsync(FileContract file)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException();
            }

            return Task.Run(() =>
            {
                FileStream fs = file.Open(FileAccess.Read);
                object res = _formatter.Deserialize(fs);
                fs.Close();
                return res;
            });
        }

    }
}
