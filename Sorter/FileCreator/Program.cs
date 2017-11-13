using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FileCreator
{
    /// <summary>
    /// Генератор файлов
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                string currentDir = Path.GetDirectoryName(currentAssembly.Location); // Определимся с текущим каталогом

                // С разбором параметров заморачиваться не стал, т.к. по заданию не требуется.
                // должно быть 3 параметра
                // имя файла (полное или нет) куда писать результат
                // ~объём целевого файла
                // максимальное количество дубликатов строк в файле
                //
                // Всё остальное параметризировать не стал, т.к. по заданию не требуется.
                if (args.Length != 3)
                {
                    WriteHelp();
                }
                else
                {
                    string fullFileName;
                    string fileName = args[0];
                    if (Path.IsPathRooted(fileName))
                    {
                        fullFileName = fileName;
                    }
                    else
                    {
                        fullFileName = Path.Combine(currentDir, fileName);
                    }

                    int targetFileSize;
                    if (!int.TryParse(args[1], out targetFileSize))
                    {
                        throw new ArgumentException("Can't parse target file size.");
                    }
                    targetFileSize = Math.Abs(targetFileSize);

                    ushort maxNumberOfDupes;
                    if (!ushort.TryParse(args[2], out maxNumberOfDupes))
                    {
                        throw new ArgumentException("Can't parse max number of dupes [0..65535].");
                    }

                    // Замерим реальное время создания файла
                    var stopwatch = Stopwatch.StartNew();
                    CreateFile(fullFileName, targetFileSize, maxNumberOfDupes);
                    stopwatch.Stop();

                    Console.WriteLine("Elapsed file generation time: {0}.", stopwatch.Elapsed);
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

        private static void WriteHelp()
        {
            Console.WriteLine("FileCreator.exe file_name target_file_size max_number_of_dupes");
        }

        private static void CreateFile(string fullFileName, int targetFileSize, ushort maxNumberOfDupes)
        {
            using (var fileProducer = new FileProducer())
            {
                fileProducer.WriteFile(fullFileName, targetFileSize, maxNumberOfDupes);
            }
        }
    }
}