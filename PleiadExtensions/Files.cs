using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace PleiadExtensions
{
    namespace Files
    {
        /// <summary>
        /// Contains filename and a lock object for file
        /// </summary>
        public struct FileContract
        {
            /// <summary>
            /// Contains filename and a lock object for file
            /// </summary>
            public FileContract(string filename)
            {
                //if (!File.Exists(filename)) File.Create(filename);
                Exists = File.Exists(filename);
                FileName = filename;
                Locker = new object();
            }
            /// <summary>
            /// Filename
            /// </summary>
            public string FileName { get; }
            public bool Exists { get; private set; }
            /// <summary>
            /// Locker object
            /// </summary>
            public object Locker { get; }

            /// <summary>
            /// Default timeout for write retry
            /// </summary>
            public const int DefaultTimeout = 100;
            private static int _timeout = DefaultTimeout;
            /// <summary>
            /// Custom timeout for write retry
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


            public void Create()
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        lock (Locker)
                        {
                            var fs = File.Create(FileName);
                            Exists = true;
                            fs.Close();

                            return;
                        }
                    }
                    catch (IOException)
                    { }

                    if (sw.ElapsedMilliseconds > _timeout)
                        break;
                    Thread.Sleep(5);
                }
                sw.Stop();
                throw new TimeoutException($"Could not create file in {sw.ElapsedMilliseconds}ms");
            }
            /// <summary>
            /// Read all lines from the file
            /// </summary>
            /// <returns>Array of strings in file/></returns>
            public List<string> ReadLines()
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        return new List<string>(File.ReadAllLines(FileName).ToArray());
                    }
                    catch
                    { }

                    if (sw.ElapsedMilliseconds > _timeout)
                        break;
                    Thread.Sleep(5);
                }
                sw.Stop();
                return new List<string>();
            }
            /// <summary>
            /// Read all lines that contain a sequence
            /// </summary>
            /// <param name="sequence">Query sequence</param>
            /// <returns>Array of strings</returns>
            public List<string> ReadContaining(string sequence)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        return new List<string>(File.ReadAllLines(FileName).Where(s => s.Contains(sequence)).ToArray());
                    }
                    catch
                    { }

                    if (sw.ElapsedMilliseconds > _timeout)
                        break;
                    Thread.Sleep(5);
                }
                sw.Stop();
                return new List<string>();
            }
            /// <summary>
            /// Append line to the file
            /// </summary>
            /// <param name="content"></param>
            public void Write(string content)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        lock (Locker)
                        {
                            File.AppendAllText(FileName, $"{content}\n");
                        }
                        break;
                    }
                    catch
                    {
                    }
                    if (sw.ElapsedMilliseconds > _timeout)
                        break;
                    Thread.Sleep(5);
                }
                sw.Stop();
            }
            /// <summary>
            /// Append lines to the file
            /// </summary>
            /// <param name="content"></param>
            public void WriteLines(string[] content)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        lock (Locker)
                        {
                            File.AppendAllLines(FileName, content);
                        }
                        break;
                    }
                    catch
                    {
                    }
                    if (sw.ElapsedMilliseconds > _timeout)
                        break;
                    Thread.Sleep(5);
                }
                sw.Stop();
            }


            #region Overrides
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

            public override bool Equals(object obj)
            {
                return obj is FileContract contract &&
                       FileName == FileName &&
                       EqualityComparer<object>.Default.Equals(Locker, Locker);
            }

            public override int GetHashCode()
            {
                int hashCode = -1944103139;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
                hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Locker);
                return hashCode;
            }

            public static bool operator ==(FileContract left, FileContract right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(FileContract left, FileContract right)
            {
                return !(left == right);
            }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
            #endregion
        }
    }
}
