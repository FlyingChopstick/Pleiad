using PleiadMisc.Files;
using System.Threading.Tasks;

namespace PleiadMisc.Serialisers
{
    public interface IPSerialiser<T>
    {
        Task<T> DeserialiseAsync(FileContract file);
        Task SerialiseAsync(T data, FileContract file);
    }
}