using System;
using System.Collections.Generic;
using System.IO;
using Sorter.ForeignCode;

namespace Sorter
{
    public class RecordExternalSorter 
        : ExternalSorter<Record>
    {
        private readonly string _tempDir;

        public RecordExternalSorter(int capacity, int mergeCount, string tempDir, Func<IComparer<Record>, IQueue<Record>> queueFactoryFunc)
            : base(new RecordComparer(), capacity, mergeCount, queueFactoryFunc)
        {
            if (tempDir == null) throw new ArgumentNullException("tempDir");

            _tempDir = tempDir;
        }

        protected override string Write(IEnumerable<Record> run)
        {
            var file = Path.Combine(_tempDir, "part" + Guid.NewGuid() + ".tmp");
            using (var writer = new StreamWriter(file))
            {
                foreach (var record in run)
                {
                    writer.WriteLine(record.GetText());
                }
            }

            return file;
        }

        protected override IEnumerable<Record> Read(string fileName)
        {
            using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var stream = new BufferedStream(input, 64 * 1024))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    int pointIndex = line.IndexOf('.');
                    int number = int.Parse(line.Substring(0, pointIndex));
                    
                    string str = line.Substring(pointIndex + 2, line.Length - pointIndex - 2);

                    yield return new Record(number, str);
                }
            }

            if (fileName.EndsWith(".tmp"))
                File.Delete(fileName);
        }
    }
}