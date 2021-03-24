using PleiadExtensions.Files;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PleiadMisc
{
    public class PSerialiser<T>
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
        public async Task<T> DeserialiseAsync(string filename)
        {
            using FileStream stream = File.OpenRead(filename);
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }
}
