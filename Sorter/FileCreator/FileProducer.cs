using System;
using System.IO;
using System.Text;
using System.Threading;

namespace FileCreator
{
    public sealed class FileProducer
        : IDisposable
    {
        // Константы, описывающие диапазоны кодов символов, которые можно использовать в строках
        private const int CharMin = 0x21;
        private const int CharMax = 0x7E;
        private const int CapitalLettersMin = 0x41;
        private const int CapitalLettersMax = 0x5A;
        private const int LettersMin = 0x61;
        private const int LettersMax = 0x7A;

        // Что такое один процент от int.Max
        private const int OnePercent = int.MaxValue / 100;

        // Псевдорандомизатор которым будем пользоваться для получения всех случайных чисел
        private readonly Random _rnd;

        // С таймером заморочился только чтобы было веселее смотреть как файл создаётся
        private Timer _timer;
        private byte _spinPosition;
        private int _currentFileSize;
        private int _targetFileSize;

        public FileProducer()
        {
            _rnd = new Random(DateTime.Now.Millisecond);
            _spinPosition = 0;
            _currentFileSize = 0;
            _timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
        }

        ~FileProducer()
        {
            Dispose();
        }

        /// <summary>
        /// Основной метод создания файла с требуемыми данными
        /// </summary>
        /// <param name="fullFileName">Полное имя файла</param>
        /// <param name="targetFileSize">Приблизительный максимальный размер файла</param>
        /// <param name="maxNumberOfDupes">Максимальное количество дублей строк</param>
        public void WriteFile(string fullFileName, int targetFileSize, int maxNumberOfDupes)
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");
            if (maxNumberOfDupes < 0 || 65535 < maxNumberOfDupes) throw new ArgumentOutOfRangeException("maxNumberOfDupes", maxNumberOfDupes, "Max number of dupes must be at range [0..65535].");

            _targetFileSize = targetFileSize;
            int nextNumber = 0;
            int lastDupeIndex = -1;

            // Дубли ограничил диапазоном [0..65535] т.к. в ТЗ требований к ним нет, лишь сказано, что строки могут повторяться.
            // Избрал путь наименьшего сопротивления
            var dupes = new StringBuilder[maxNumberOfDupes];
            int currentNumberOfDupes = 0;

            // Таймер позволит наблюдать прогресс создания файла
            _timer.Change(100, 100);
            try
            {
                // Файловый поток, над которым сидит буфферизованный поток и потоковый писальщик.
                // Всё это нужно чтобы процесс записи на диск был максимально гладким
                using (var fileStream = new FileStream(fullFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                using (var bufferedStream = new BufferedStream(fileStream, 10 * 1024 * 1024))
                using (var writer = new StreamWriter(bufferedStream, Encoding.ASCII))
                {
                    while (true)
                    {
                        // Приблизительно в 3% случаев будем выбирать дублирующую строку из ранее отобранных в дубли строк
                        StringBuilder nextLine;
                        if (0 <= lastDupeIndex && currentNumberOfDupes < maxNumberOfDupes && _rnd.Next() < OnePercent * 3)
                        {
                            // Дублирующей строке надо отпилить её счётчик и дать новый, можно было сразу пилить при помещении в дубли, но серьёзной разницы нет
                            nextLine = GetNextLine(++nextNumber, dupes[_rnd.Next(0, lastDupeIndex)]);
                            currentNumberOfDupes++;
                        }
                        else
                        {
                            // В общем случае генерируем новую строку для записи. В StringBuilder это всё делается чтобы избежать постоянных копирований строк
                            // в памяти при входе/выходе из методов, присвоениях и т.п.
                            nextLine = GetNextLine(++nextNumber, (byte)_rnd.Next(1, 2), (byte)_rnd.Next(2, 10), (byte)_rnd.Next(1, 5));
                        }
                        
                        writer.Write(nextLine);

                        // Ведём подсчёт получаещегося размера файла и если он становится выше целевого, то завершаем файл
                        _currentFileSize += nextLine.Length;
                        if (_currentFileSize > targetFileSize)
                            break;

                        writer.WriteLine();

                        _currentFileSize += 2;

                        // Приблизительно в 3% случаев выберем эту строку в потенциальные дубликаты, если ещё есть место
                        if (lastDupeIndex < maxNumberOfDupes - 1 &&_rnd.Next() < OnePercent * 3)
                        {
                            lastDupeIndex++;
                            dupes[lastDupeIndex] = nextLine;
                        }
                    }
                }
            }
            finally
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);

                _currentFileSize = _targetFileSize;
                Spin();
                Console.WriteLine();
                Console.WriteLine("Line count: {0}.", nextNumber);
                Console.WriteLine("Dupes count: {0}.", currentNumberOfDupes);
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

        /// <summary>
        /// Стандартная визуализация процесса генерации файла
        /// </summary>
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

        /// <summary>
        /// Возвращает новую строку, на основе уже имеющейся (дубликат). У новой строки цифровой индекс заменён.
        /// </summary>
        /// <param name="nextNumber"></param>
        /// <param name="duplicate"></param>
        /// <returns></returns>
        private StringBuilder GetNextLine(int nextNumber, StringBuilder duplicate)
        {
            for (int charIndex = 0; charIndex < duplicate.Length; charIndex++)
            {
                if (duplicate[charIndex] == '.')
                {
                    char[] chars = new char[duplicate.Length - charIndex - 2];
                    duplicate.CopyTo(charIndex + 2, chars, 0, duplicate.Length - charIndex - 2);

                    var sb = new StringBuilder(nextNumber + ". ");
                    sb.Append(chars);

                    return sb;
                }
            }

            throw new InvalidOperationException(string.Format("Can't find char '.' in string '{0}'.", duplicate));
        }

        /// <summary>
        /// Возвращает новую строку
        /// </summary>
        /// <param name="nextNumber">Цифровой индекс</param>
        /// <param name="wordMinLength">Минимальное количество букв в словах</param>
        /// <param name="wordMaxLength">Максимальное количество букв в словах</param>
        /// <param name="maxWords">Количество слов</param>
        /// <returns></returns>
        private StringBuilder GetNextLine(int nextNumber, byte wordMinLength, byte wordMaxLength, byte maxWords)
        {
            var sb = new StringBuilder(nextNumber + ". ");
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

        /// <summary>
        /// Возвращает следующее слово в виде массива char[], это несколько производительнее чем гонять строку или StringBulder
        /// </summary>
        /// <param name="fromCapital">Начинать ли слово с заглавной буквы</param>
        /// <param name="wordMinLength">Минимальное количество букв в словах</param>
        /// <param name="wordMaxLength">Максимальное количество букв в словах</param>
        /// <param name="withNonLetters">Чило, указывающее на факт допустимости использовать не словесные символы</param>
        /// <returns></returns>
        private char[] GetNextWord(bool fromCapital, byte wordMinLength, byte wordMaxLength, int withNonLetters)
        {
            int length = _rnd.Next(wordMinLength, wordMaxLength);
            char[] result = new char[length];

            unchecked
            {
                for (int charIndex = 0; charIndex < length; charIndex++)
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
                        // Приблизительно в 1% слов будут использованы не только словесные символы
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

            return result;
        }
    }
}