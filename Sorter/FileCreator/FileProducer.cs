using System;
using System.IO;
using System.Text;
using System.Threading;

namespace FileCreator
{
    public sealed class FileProducer
        : IDisposable
    {
        private const int CharMin = 0x21;
        private const int CharMax = 0x7E;
        private const int CapitalLettersMin = 0x41;
        private const int CapitalLettersMax = 0x5A;
        private const int LettersMin = 0x61;
        private const int LettersMax = 0x7A;

        private const int OnePercent = int.MaxValue / 100;

        private readonly Random _rnd;
        
        private Timer _timer;
        private byte _spinPosition;
        private uint _currentFileSize;
        private uint _targetFileSize;
        private uint _nextNumber;

        public FileProducer()
        {
            _rnd = new Random(DateTime.Now.Millisecond);
            _spinPosition = 0;
            _currentFileSize = 0;
            _nextNumber = 0;
            _timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
        }

        ~FileProducer()
        {
            Dispose();
        }

        public void WriteFile(string fullFileName, uint targetFileSize, uint numberOfTextDuplicates)
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");

            _targetFileSize = targetFileSize;
            _nextNumber = 0;

            _timer.Change(100, 100);
            try
            {
                using (var fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var bufferedStream = new BufferedStream(fileStream, 1000 * 1024))
                using (var writer = new StreamWriter(bufferedStream, Encoding.ASCII))
                {
                    while (true)
                    {
                        uint nextNumber = GetNextNumber();
                        var nextText = GetNextText((byte)_rnd.Next(1, 2), (byte)_rnd.Next(2, 10), (byte)_rnd.Next(1, 5));

                        writer.Write(nextNumber);
                        writer.Write(". ");
                        writer.Write(nextText);

                        _currentFileSize += (uint)(IntLength(nextNumber) + 2 + nextText.Length);

                        if (_currentFileSize > targetFileSize)
                            break;

                        writer.WriteLine();

                        _currentFileSize += 2;
                        ////writer.Flush();
                        ////bufferedStream.Flush();
                        ////fileStream.Flush(true);
                        
                        //Thread.Sleep(100);
                    }
                }
            }
            finally
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                _currentFileSize = _targetFileSize;
                Spin();
                Console.WriteLine();
            }
        }

        public void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();

            _timer = null;
        }

        private void OnTimer(object state)
        {
            Spin();
        }

        private void Spin()
        {
            if (_currentFileSize < _targetFileSize)
            {
                _spinPosition++;
                switch (_spinPosition % 4)
                {
                    case 0:
                        Console.Write("/");
                        break;
                    case 1:
                        Console.Write("-");
                        break;
                    case 2:
                        Console.Write("\\");
                        break;
                    case 3:
                        Console.Write("|");
                        break;
                }
            }
            else
            {
                Console.Write(" ");
            }

            Console.Write(" {0:P}", ((double)_currentFileSize / (double)_targetFileSize));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        private static int IntLength(uint i)
        {
            if (i < 10)
                return 1;

            return (int)Math.Floor(Math.Log10(i)) + 1;
        }

        private uint GetNextNumber()
        {
            return ++_nextNumber;
            ////return (uint)_rnd.Next(int.MinValue, int.MaxValue);
        }

        private StringBuilder GetNextText(byte wordMinLength, byte wordMaxLength, byte maxWords)
        {
            var sb = new StringBuilder();
            int words = 0;
            while (words < maxWords)
            {
                words++;
                sb.Append(GetNextWord(words == 1, wordMinLength, wordMaxLength, _rnd.Next()));

                if (words < maxWords)
                    sb.Append(" ");
            }

            return sb;
        }

        private string GetNextWord(bool fromCapital, byte wordMinLength, byte wordMaxLength, int withNonLetters)
        {
            int length = _rnd.Next(wordMinLength, wordMaxLength);
            char[] result = new char[length];

            unchecked
            {
                for (int charIndex = 0; charIndex < result.Length; charIndex++)
                {
                    if (charIndex == 0)
                    {
                        if (fromCapital)
                        {
                            result[charIndex] = (char)_rnd.Next(CapitalLettersMin, CapitalLettersMax);
                        }
                        else
                        {
                            result[charIndex] = (char)_rnd.Next(LettersMin, LettersMax);
                        }
                    }
                    else
                    {
                        if (withNonLetters < OnePercent)
                        {
                            result[charIndex] = (char)_rnd.Next(CharMin, CharMax);
                        }
                        else
                        {
                            result[charIndex] = (char)_rnd.Next(LettersMin, LettersMax);
                        }
                    }
                }
            }

            return new string(result);
        }
    }
}