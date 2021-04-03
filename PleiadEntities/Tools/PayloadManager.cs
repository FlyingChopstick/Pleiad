using PleiadMisc.Files;
using PleiadMisc.Serialisers;
using System.Threading.Tasks;

namespace PleiadEntities.Tools
{
    public class PayloadManager<T> where T : IPleiadComponent
    {
        IPSerialiser<Payload<T>> _s;

        public PayloadManager(IPSerialiser<Payload<T>> pSerialiser)
        {
            _s = pSerialiser;
        }


        public async Task SaveAsync(Payload<T> payload, FileContract saveFile)
        {
            await _s.SerialiseAsync(payload, saveFile);
        }

        public async Task<Payload<T>> LoadAsync(FileContract file)
        {
            return await _s.DeserialiseAsync(file);
        }
    }
}
