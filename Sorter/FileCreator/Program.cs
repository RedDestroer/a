using System;
using System.IO;
using System.Reflection;

namespace FileCreator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                string currentDir = Path.GetDirectoryName(currentAssembly.Location);

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

                    uint targetFileSize;
                    if (!uint.TryParse(args[1], out targetFileSize))
                    {
                        throw new ArgumentException("Can't parse target file size.");
                    }

                    uint numberOfTextDuplicates;
                    if (!uint.TryParse(args[1], out numberOfTextDuplicates))
                    {
                        throw new ArgumentException("Can't parse number of string dupes.");
                    }

                    CreateFile(fullFileName, targetFileSize, numberOfTextDuplicates);
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
            Console.WriteLine("FileCreator file_name target_file_size number_of_string_dupes");
        }

        private static void CreateFile(string fullFileName, uint targetFileSize, uint numberOfTextDuplicates)
        {
            using (var fileProducer = new FileProducer())
            {
                fileProducer.WriteFile(fullFileName, targetFileSize, numberOfTextDuplicates);
            }
        }
    }
}