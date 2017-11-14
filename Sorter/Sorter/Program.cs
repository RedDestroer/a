using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sorter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                string currentDir = Path.GetDirectoryName(currentAssembly.Location); // Определимся с текущим каталогом

                if (args.Length != 3)
                {
                    WriteHelp();
                }
                else
                {
                    string inputFullFileName = ExpandFileName(currentDir, args[0]);
                    string outputFullFileName = ExpandFileName(currentDir, args[1]);

                    ulong maxSortTreeNodes;
                    if (!ulong.TryParse(args[2], out maxSortTreeNodes))
                    {
                        throw new ArgumentException("Can't parse max sort tree nodes.");
                    }

                    // Замерим реальное время создания файла
                    var stopwatch = Stopwatch.StartNew();
                    OuterMergeSort(inputFullFileName, outputFullFileName, maxSortTreeNodes);
                    stopwatch.Stop();

                    Console.WriteLine("Elapsed sort time: {0}.", stopwatch.Elapsed);
                }

                Environment.ExitCode = 0;
            }
            catch (ArgsException exception)
            {
                Console.WriteLine(exception.Message);

                Environment.ExitCode = 1;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);

                Environment.ExitCode = 1;
            }

#if DEBUG
            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
#endif
        }

        private static void OuterMergeSort(string inputFullFileName, string outputFullFileName, ulong maxSortTreeNodes)
        {
            var sorter = new RecordExternalSorter(1000, 100, @"W:\Temp\Temp");
            var sortedFileName = sorter.Sort(inputFullFileName);

            if (File.Exists(outputFullFileName))
                File.Delete(outputFullFileName);

            File.Move(sortedFileName, outputFullFileName);
        }

        private static string ExpandFileName(string currentDir, string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            }

            return Path.Combine(currentDir, fileName);
        }

        private static void WriteHelp()
        {
            Console.WriteLine("Sorter.exe input_file_name output_file_size max_sort_tree_nodes");
        }
    }
}
