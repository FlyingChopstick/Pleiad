using System.IO;

namespace Pleiad.Extensions
{
    namespace Files
    {
        /// <summary>
        /// A structure to represent the file on the device
        /// </summary>
        public struct FileContract
        {
            /// <summary>
            /// Creates the file if it does not exist
            /// </summary>
            /// <param name="filename">File</param>
            public FileContract(string filename)
            {
                if (!File.Exists(filename)) File.Create(filename);
                FileName = filename;
            }
            public string FileName { get; init; }
        }
    }
}
