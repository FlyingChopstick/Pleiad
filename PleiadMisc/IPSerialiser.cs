using PleiadExtensions.Files;
using System.Threading.Tasks;

namespace PleiadMisc
{
    public interface IPSerialiser<T>
    {
        Task<T> DeserialiseAsync(FileContract file);
        Task SerialiseAsync(T data, FileContract file);
    }
}