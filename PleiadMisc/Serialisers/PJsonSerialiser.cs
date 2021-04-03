using PleiadMisc.Files;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PleiadMisc.Serialisers
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
            return Task.Run(async () =>
            {
                using FileStream fs = file.Open(FileAccess.Write);
                await JsonSerializer.SerializeAsync(fs, data);
            });
        }
        public async Task<T> DeserialiseAsync(FileContract file)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException();
            }

            return await Task.Run(async () =>
            {
                using FileStream stream = File.OpenRead(file.FileName);
                return await JsonSerializer.DeserializeAsync<T>(stream);
            });

        }
    }
}
