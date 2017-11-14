using System;
using System.Collections.Generic;
using System.IO;

namespace Sorter
{
    public class RecordExternalSorter 
        : ExternalSorter<Record>
    {
        private readonly string _tempDir;

        public RecordExternalSorter(int capacity, int mergeCount, string tempDir)
            : base(new RecordComparer(), capacity, mergeCount)
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
                    writer.WriteLine("{0}. {1}", record.Number, record.Str);
                }
            }

            return file;
        }

        protected override IEnumerable<Record> Read(string name)
        {
            using (var reader = new StreamReader(name))
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

            if (name.EndsWith(".tmp"))
                File.Delete(name);
        }
    }
}