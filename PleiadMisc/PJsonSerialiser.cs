using PleiadExtensions.Files;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;

namespace PleiadMisc
{
    public class PJsonSerialiser<T> : IPSerialiser<T>
    {
        public Task SerialiseAsync(T data, FileContract file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file.FileName));
            if (!file.Exists)
            {
                file.Create();
            }
            using FileStream fs = File.OpenWrite(file.FileName);
            return JsonSerializer.SerializeAsync(fs, data);
        }
        public async Task<T> DeserialiseAsync(FileContract file)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException();
            }
            using FileStream stream = File.OpenRead(file.FileName);
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }
    public class PSerialiser<T> : IPSerialiser<T>
    {
        BinaryFormatter _formatter = new();

        public Task SerialiseAsync(T data, FileContract file)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file.FileName));
            return Task.Run(() =>
            {
                FileStream fs = file.Open(FileAccess.Write);
                _formatter.Serialize(fs, data);
                fs.Close();
            });
        }
        public Task<T> DeserialiseAsync(FileContract file)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException();
            }
            return Task.Run(() =>
            {
                FileStream fs = file.Open(FileAccess.Read);
                T res = (T)_formatter.Deserialize(fs);
                fs.Close();
                return res;
            });
        }

    }
}
