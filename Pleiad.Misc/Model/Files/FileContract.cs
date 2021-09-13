using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pleiad.Misc.Model.Files;

namespace Pleiad.Common.Model.Files
{
    /// <summary>
    /// A structure to represent the file on the device
    /// </summary>
    public class FileContract
    {
        /// <summary>
        /// Creates the file if it does not exist
        /// </summary>
        /// <param name="filename">File</param>
        public FileContract(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                System.IO.File.Create(filename);
            }
            Filepath = new(filename);
        }
        /// <summary>
        /// Creates the file if it does not exist
        /// </summary>
        /// <param name="filename">File</param>
        public FileContract(FilePath file)
        {
            if (!System.IO.File.Exists(file))
            {
                System.IO.File.Create(file);
            }
            Filepath = file;
        }
        public FilePath Filepath { get; init; }


        /// <summary>
        /// Default timeout of file access operation
        /// </summary>
        public const int DefaultTimeout = 100;
        /// <summary>
        /// Get or set the time to access the file. Throws <see cref="ArgumentException"/> if <0
        /// </summary>
        public int Timeout
        {
            get => _timeout;
            set
            {
                if (value < 0) throw new ArgumentException($"Timeout must be >= 0 (tried {value}).");
                _timeout = value;
            }
        }


        public string[] ReadLines()
        {
            if (_lock.TryEnterReadLock(_timeout))
            {
                string[] content = Array.Empty<string>();
                try
                {
                    content = File.ReadAllLines(Filepath).ToArray();
                }
                catch { }
                finally
                {
                    _lock.ExitReadLock();
                }

                return content;
            }
            else throw GetFileAccessException(this, FileAccess.Read);
        }
        /// <summary>
        /// Reads the content of a file
        /// </summary>
        /// <returns>Content of the file</returns>
        public async Task<string[]> ReadLinesAsync()
        {
            return await Task.Run(() => ReadLines());
        }

        /// <summary>
        /// Read all lines containing the query from the file
        /// </summary>
        /// <param name="sequence">Query string</param>
        /// <returns>Lines containing the query</returns>
        public async Task<string[]> ReadContainingAsync(string sequence)
        {
            return await Task.Run(() =>
            {
                if (_lock.TryEnterReadLock(_timeout))
                {
                    string[] content = Array.Empty<string>();
                    try
                    {
                        content = File.ReadAllLines(Filepath).Where(s => s.Contains(sequence)).ToArray();
                    }
                    catch { }
                    finally
                    {
                        _lock.ExitReadLock();
                    }

                    return content;
                }
                else throw GetFileAccessException(this, FileAccess.Read);
            });
        }


        /// <summary>
        /// Write a line to a file on a background thread
        /// </summary>
        /// <param name="content">Line to write</param>
        public void Write(string content)
        {
            Thread writeThread = new(t =>
            {
                if (_lock.TryEnterWriteLock(_timeout))
                {
                    try
                    {
                        System.IO.File.AppendAllText(Filepath, $"{content}\n");
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
                else throw GetFileAccessException(this, FileAccess.Write);
            });

            writeThread.Name = "WriteThread";
            writeThread.IsBackground = true;
            writeThread.Start();
        }
        /// <summary>
        /// Write several lines to a file on a background thread
        /// </summary>
        /// <param name="content">Lines to write</param>
        public void WriteLines(string[] content)
        {
            Thread writeThread = new(t =>
            {
                if (_lock.TryEnterWriteLock(_timeout))
                {
                    try
                    {

                        System.IO.File.AppendAllLines(Filepath, content);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
                else throw GetFileAccessException(this, FileAccess.Write);
            });

            writeThread.Name = "WriteBatchThread";
            writeThread.IsBackground = true;
            writeThread.Start();
        }



        private static IOException GetFileAccessException(FileContract file, FileAccess operation)
        {
            string fileMode = operation switch
            {
                FileAccess.Read => "reading",
                FileAccess.Write => "writing",
                FileAccess.ReadWrite => "read/write",
                _ => throw new ArgumentException($"Bad argumemnt provided: {operation}")
            };
            return new IOException($"Could not access the file {file.Filepath} for {fileMode} in {file.Timeout}ms");
        }

        private readonly ReaderWriterLockSlim _lock = new();
        private int _timeout = DefaultTimeout;
    }
}
