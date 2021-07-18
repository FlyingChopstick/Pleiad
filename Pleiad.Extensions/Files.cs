using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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


        /// <summary>
        /// Static class containing different file operations
        /// </summary>
        public static class Files
        {
            /// <summary>
            /// Default timeout of file access operation
            /// </summary>
            public const int DefaultTimeout = 100;
            /// <summary>
            /// Get or set the time to access the file. Throws <see cref="ArgumentException"/> if <0
            /// </summary>
            public static int Timeout
            {
                get => _timeout;
                set
                {
                    if (value < 0) throw new ArgumentException($"Timeout must be >= 0 (tried {value}).");
                    _timeout = value;
                }
            }


            public static string[] ReadLines(this FileContract contract)
            {
                if (_lock.TryEnterReadLock(_timeout))
                {
                    string[] content = Array.Empty<string>();
                    try
                    {
                        content = File.ReadAllLines(contract.FileName).ToArray();
                    }
                    catch { }
                    finally
                    {
                        _lock.ExitReadLock();
                    }

                    return content;
                }
                else throw GetFileAccessException(contract.FileName, FileAccess.Read);
            }
            /// <summary>
            /// Reads the content of a file
            /// </summary>
            /// <param name="contract">Target file</param>
            /// <returns>Content of the file</returns>
            public static async Task<string[]> ReadLinesAsync(this FileContract contract)
            {
                return await Task.Run(() =>
                {
                    return ReadLines(contract);
                });
            }
            /// <summary>
            /// Read all lines containing the query from the file
            /// </summary>
            /// <param name="contract">Target file</param>
            /// <param name="sequence">Query string</param>
            /// <returns>Lines containing the query</returns>
            public static async Task<string[]> ReadContainingAsync(this FileContract contract, string sequence)
            {
                return await Task.Run(() =>
                {
                    if (_lock.TryEnterReadLock(_timeout))
                    {
                        string[] content = Array.Empty<string>();
                        try
                        {
                            content = File.ReadAllLines(contract.FileName).Where(s => s.Contains(sequence)).ToArray();
                        }
                        catch { }
                        finally
                        {
                            _lock.ExitReadLock();
                        }

                        return content;
                    }
                    else throw GetFileAccessException(contract.FileName, FileAccess.Read);
                });
            }


            /// <summary>
            /// Write a line to a file on a background thread
            /// </summary>
            /// <param name="contract">Target file</param>
            /// <param name="content">Line to write</param>
            public static void Write(this FileContract contract, string content)
            {
                Thread writeThread = new(t =>
                {
                    if (_lock.TryEnterWriteLock(_timeout))
                    {
                        try
                        {
                            File.AppendAllText(contract.FileName, $"{content}\n");
                        }
                        catch
                        {
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                    else throw GetFileAccessException(contract.FileName, FileAccess.Write);
                });

                writeThread.Name = "WriteThread";
                writeThread.IsBackground = true;
                writeThread.Start();
            }
            /// <summary>
            /// Write several lines to a file on a background thread
            /// </summary>
            /// <param name="contract">Target file</param>
            /// <param name="content">Lines to write</param>
            public static void WriteLines(this FileContract contract, string[] content)
            {
                Thread writeThread = new(t =>
                {
                    if (_lock.TryEnterWriteLock(_timeout))
                    {
                        try
                        {

                            File.AppendAllLines(contract.FileName, content);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            _lock.ExitWriteLock();
                        }
                    }
                    else throw GetFileAccessException(contract.FileName, FileAccess.Write);
                });

                writeThread.Name = "WriteBatchThread";
                writeThread.IsBackground = true;
                writeThread.Start();
            }



            private static IOException GetFileAccessException(string filename, FileAccess operation)
            {
                string fileMode = operation switch
                {
                    FileAccess.Read => "reading",
                    FileAccess.Write => "writing",
                    FileAccess.ReadWrite => "read/write",
                    _ => throw new ArgumentException($"Bad argumemnt provided: {operation}")
                };
                return new IOException($"Could not access the file {filename} for {fileMode} in {Timeout}ms");
            }

            private static ReaderWriterLockSlim _lock = new();
            private static int _timeout = DefaultTimeout;
        }
    }
}
