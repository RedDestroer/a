using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Sorter.ForeignCode;

namespace Sorter
{
    /// <summary>
    /// Вся доп. информация размещена в файле Readme.txt
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                string currentDir = Path.GetDirectoryName(currentAssembly.Location); // Определимся с текущим каталогом

                if (args.Length != 6)
                {
                    WriteHelp();
                }
                else
                {
                    string inputFullFileName = ExpandFileName(currentDir, args[0]);
                    string outputFullFileName = ExpandFileName(currentDir, args[1]);
                    int capacity = GetAbsInt(args[2], "Can't parse max sort tree nodes.");
                    int mergeCount = GetAbsInt(args[3], "Can't parse merge count.");
                    int sorterType = GetAbsInt(args[4], "Can't parse sorter type.");
                    string tempDir = ExpandFileName(currentDir, args[5]);

                    Console.WriteLine("Sort in progress...");

                    // Замерим реальное время создания файла
                    var stopwatch = Stopwatch.StartNew();
                    OuterMergeSort(inputFullFileName, outputFullFileName, capacity, mergeCount, sorterType, tempDir);
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

        private static void OuterMergeSort(string inputFullFileName, string outputFullFileName, int capacity, int mergeCount, int sorterType, string tempDir)
        {
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            string sortedFileName;
            switch (sorterType)
            {
                case 0:
                    sortedFileName = SorterType0(inputFullFileName, capacity, mergeCount, tempDir);
                    break;
                case 1:
                    sortedFileName = SorterType1(inputFullFileName, capacity, mergeCount, tempDir);
                    break;
                case 2:
                    sortedFileName = SorterType2(inputFullFileName, capacity, mergeCount, tempDir);
                    break;
                default:
                    throw new ArgsException("Unknown sorter type:{0}. Expected 0 or 1.");
            }
            
            if (File.Exists(outputFullFileName))
                File.Delete(outputFullFileName);

            File.Move(sortedFileName, outputFullFileName);
        }

        private static string SorterType0(string inputFullFileName, int capacity, int mergeCount, string tempDir)
        {
            var sorter = new RecordExternalSorter(capacity, mergeCount, tempDir, comparer => new PriorityQueue<Record>(comparer, capacity));

            return sorter.Sort(inputFullFileName);
        }

        private static string SorterType1(string inputFullFileName, int capacity, int mergeCount, string tempDir)
        {
            var sorter = new OrdinalRecordExternalSorter(capacity, mergeCount, tempDir, comparer => new PriorityQueue<OrdinalRecord>(comparer, capacity));

            return sorter.Sort(inputFullFileName);
        }

        private static string SorterType2(string inputFullFileName, int capacity, int mergeCount, string tempDir)
        {
            var sorter = new RecordExternalSorter(capacity, mergeCount, tempDir, comparer => new FibonacciPriorityQueue<Record>(capacity, Record.Min));

            return sorter.Sort(inputFullFileName);
        }

        private static string ExpandFileName(string currentDir, string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            }

            return Path.Combine(currentDir, fileName);
        }

        private static int GetAbsInt(string value, string errorMessage)
        {
            int result;
            if (!int.TryParse(value, out result))
            {
                throw new ArgumentException(errorMessage);
            }

            return Math.Abs(result);
        }

        private static void WriteHelp()
        {
            Console.WriteLine("Sorter.exe input_file_name output_file_name capacity merge_count sorter_type temp_dir");
            Console.WriteLine(" input_file_name - full file name of file in question");
            Console.WriteLine(" output_file_name - full file name of file with sort result");
            Console.WriteLine(" capacity - number of elements in queues in memory, larger number eger for more memory consumption");
            Console.WriteLine(" merge_count - number of queues for merge, larger number produce more queues");
            Console.WriteLine(" sorter_type - 0-Alphanumeric sorter, 1-Ordinal sorter, 2-Alphanumeric sorter on priority queue based on Fibonacci heap");
            Console.WriteLine(" temp_dir - directory, where to put temporary files");
        }
    }
}
