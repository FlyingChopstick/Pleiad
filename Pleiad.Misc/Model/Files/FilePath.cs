using System;
using Pleiad.Common.Model;

namespace Pleiad.Misc.Model.Files
{
    public class FilePath : ValueClass<string>
    {
        public FilePath(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("File path cannot be null");
            }

            Value = filepath;
        }
    }
}
