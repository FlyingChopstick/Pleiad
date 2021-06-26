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
        public struct FileContract
        {
            public FileContract(string filename)
            {
                if (!File.Exists(filename)) File.Create(filename);
                FileName = filename;
                Locker = new object();
            }
            public string FileName { get; }
            public object Locker { get; }

            #region Overrides
            public override bool Equals(object obj)
            {
                return obj is FileContract contract &&
                       FileName == contract.FileName &&
                       EqualityComparer<object>.Default.Equals(Locker, contract.Locker);
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
            #endregion
        }

        public static class Files
        {
            public const int DefaultTimeout = 100;
            private static int _timeout = DefaultTimeout;
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
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        return File.ReadAllLines(contract.FileName).ToArray();
                    }
                    catch
                    { }

                    if (sw.ElapsedMilliseconds > _timeout)
                        break;
                    Thread.Sleep(5);
                }
                sw.Stop();
                return Array.Empty<string>();
            }
            public static string[] ReadContaining(this FileContract contract, string sequence)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        return File.ReadAllLines(contract.FileName).Where(s => s.Contains(sequence)).ToArray();
                    }
                    catch
                    { }

                    if (sw.ElapsedMilliseconds > _timeout)
                        break;
                    Thread.Sleep(5);
                }
                sw.Stop();
                return Array.Empty<string>();
            }
            public static void Write(this FileContract contract, string content)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        lock (contract.Locker)
                        {
                            File.AppendAllText(contract.FileName, $"{content}\n");
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
            public static void WriteLines(this FileContract contract, string[] content)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    try
                    {
                        lock (contract.Locker)
                        {
                            File.AppendAllLines(contract.FileName, content);
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
        }
    }
}
