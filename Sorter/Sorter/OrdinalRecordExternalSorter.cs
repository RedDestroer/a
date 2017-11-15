using System;
using System.Collections.Generic;
using System.IO;
using Sorter.ForeignCode;

namespace Sorter
{
    public class OrdinalRecordExternalSorter
        : ExternalSorter<OrdinalRecord>
    {
        private readonly string _tempDir;

        public OrdinalRecordExternalSorter(int mergeCount, string tempDir, Func<IComparer<OrdinalRecord>, IQueue<OrdinalRecord>> queueFactoryFunc)
            : base(new OrdinalRecordComparer(), mergeCount, queueFactoryFunc)
        {
            if (tempDir == null) throw new ArgumentNullException("tempDir");

            _tempDir = tempDir;
        }

        protected override string Write(IEnumerable<OrdinalRecord> run)
        {
            var file = Path.Combine(_tempDir, "part" + Guid.NewGuid() + ".tmp");
            using (var input = new FileStream(file, FileMode.Create, FileAccess.Write))
            using (var stream = new BufferedStream(input, 64 * 1024))
            using (var writer = new StreamWriter(stream))
            {
                foreach (var record in run)
                {
                    writer.WriteLine(record.GetText());
                }
            }

            return file;
        }

        protected override IEnumerable<OrdinalRecord> Read(string fileName)
        {
            byte[] buffer = new byte[64 * 1024];
            int pointIndex = -1;
            int charIndex = 0;
            using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var stream = new BufferedStream(input, 64 * 1024))
            {
                while (true)
                {
                    int theByte = stream.ReadByte();
                    if (theByte < 0)
                        break;

                    if (theByte == 10 || theByte == 13)
                    {
                        if (pointIndex > -1)
                        {
                            int number = 0;
                            for (int i = pointIndex - 1; i >= 0; i--)
                            {
                                number += (buffer[i] - '0') * (int)Math.Pow(10, pointIndex - 1 - i);
                            }

                            char[] str = new char[charIndex - pointIndex - 2];
                            Array.Copy(buffer, pointIndex + 2, str, 0, charIndex - pointIndex - 2);

                            yield return new OrdinalRecord(number, str);
                        }

                        pointIndex = -1;
                        charIndex = 0;
                    }
                    else
                    {
                        if (pointIndex == -1 && theByte == 46)
                        {
                            pointIndex = charIndex;
                        }

                        buffer[charIndex] = (byte)theByte;
                        charIndex++;
                    }
                }
            }

            if (fileName.EndsWith(".tmp"))
                File.Delete(fileName);
        }
    }
}