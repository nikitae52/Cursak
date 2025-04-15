namespace ClassLibrary1
{
    public class Class1
    {
        static void Main(string[] args)
        {
            string fileName;
            string tempfile01 = "tempfile01.txt"; // Writer 1
            string tempfile02 = "tempfile02.txt"; // Writer 2
            string tempfile03 = "tempfile03.txt"; // Writer 2

            Console.WriteLine("Enter file name:");
            fileName = Console.ReadLine();

            using (StreamReader reader = new StreamReader(fileName))
            using (StreamWriter writer1 = new StreamWriter(tempfile01))
            using (StreamWriter writer2 = new StreamWriter(tempfile02))
            {
                int fib1 = 1, fib2 = 1;
                int activeFib1 = fib1, activeFib2 = fib2;
                int totalWritten1 = 0, totalWritten2 = 0;

                string line = reader.ReadLine();
                if (!int.TryParse(line, out int prevNumb)) return;

                StreamWriter activeWriter = writer1;
                bool writingToFirst = true; // true = writer1, false = writer2

                activeWriter.Write(prevNumb);
                while ((line = reader.ReadLine()) != null)
                {
                    if (!int.TryParse(line, out int number)) continue;

                    if (number >= prevNumb)
                    {
                        activeWriter.Write(" " + number);
                    }
                    else
                    {
                        activeWriter.WriteLine(); // закончился подмассив

                        // учёт подмассивов
                        if (writingToFirst) totalWritten1++;
                        else totalWritten2++;

                        // проверка: достигли ли лимита
                        if (totalWritten1 >= activeFib1 && totalWritten2 >= activeFib2)
                        {
                            int next = fib1 + fib2;
                            fib1 = fib2;
                            fib2 = next;
                            activeFib1 = fib1;
                            activeFib2 = fib2;

                        }

                        // переключаемся на нужный поток
                        if (totalWritten1 < activeFib1)
                        {
                            activeWriter = writer1;
                            writingToFirst = true;
                        }
                        else if (totalWritten2 < activeFib2)
                        {
                            activeWriter = writer2;
                            writingToFirst = false;
                        }

                        // начинаем новый подмассив
                        activeWriter.Write(number);
                    }

                    prevNumb = number;
                }

                activeWriter.WriteLine();
                if (writingToFirst) totalWritten1++;
                else totalWritten2++;

                // Дописываем недостающие EMPTY
                while (totalWritten1 < activeFib1)
                {
                    writer1.WriteLine("E");
                    totalWritten1++;
                }

                while (totalWritten2 < activeFib2)
                {
                    writer2.WriteLine("E");
                    totalWritten2++;
                }

                activeWriter.WriteLine(); // Завершить последний подмассив
            }

            MergeFiles(tempfile01, tempfile02, tempfile03);

            Console.WriteLine("Подмассивы распределены без накопления в памяти.");
        }

        static void MergeFiles(string inputfile1, string inputfile2, string outputfile)
        {
            bool has1, has2;//обьявляем тут чтобы потом анализировать для рекурсии
            using (StreamReader reader1 = new StreamReader(inputfile1))
            using (StreamReader reader2 = new StreamReader(inputfile2))
            using (StreamWriter writer = new StreamWriter(outputfile))
            {
                has1 = TryReadNextNumber(reader1, out int number1, out bool isEmptyMarker1);
                has2 = TryReadNextNumber(reader2, out int number2, out bool isEmptyMarker2);


                while (has1 && has2)
                {
                    if (number1 == int.MaxValue && number2 == int.MaxValue)
                    {
                        writer.WriteLine('E');
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                    else if (number1 == int.MaxValue && number2 != int.MaxValue)
                    {
                        while (!isEmptyMarker2)
                        {
                            writer.Write(number2 + " ");
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }
                        writer.Write(number2 + " ");
                        writer.WriteLine();
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                    else if (number1 != int.MaxValue && number2 == int.MaxValue)
                    {
                        while (!isEmptyMarker1)
                        {
                            writer.Write(number1 + " ");
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        }
                        writer.Write(number1 + " ");
                        writer.WriteLine();
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                    else
                    {
                        if (number1 <= number2)
                        {

                            if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                            {
                                writer.Write(number1 + " ");//вводим меньшеее число з 1 файла
                                writer.Write(number2 + " ");//вводим большее число з 2 файла
                                writer.WriteLine();//переходим на новую строку
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);

                            }

                            else if (isEmptyMarker1)// если число из 1 файла последнее в строке
                            {
                                writer.Write(number1 + " ");// записываем его
                                while (!isEmptyMarker2)// пишем числа из файла 2 пока строчка не закончиться
                                {
                                    writer.Write(number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                }
                                writer.Write(number2 + " ");// пишем последнее число з 2 файла
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                            {

                                while (!isEmptyMarker1) //пока не закончится строка в 1 файле
                                {
                                    if (number1 <= number2) // если  число из 1 файла меньше, выводим числа из 1 файла
                                    {
                                        writer.Write(number1 + " ");
                                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);

                                        if (isEmptyMarker1 && number1 <= number2)//если последнее число из 1 файла так и осталось меньше
                                        {
                                            writer.Write(number1 + " ");
                                            writer.Write(number2 + " ");
                                            //writer.WriteLine();
                                        }
                                        else if (isEmptyMarker1 && number1 > number2)//если последнее число из 1 файла стало больше
                                        {
                                            writer.Write(number2 + " ");
                                            writer.Write(number1 + " ");
                                            //writer.WriteLine();
                                        }
                                    }
                                    else //пока они не станут больше
                                    {
                                        writer.Write(number2 + " ");// пишем последнее число з 2 файла
                                        while (!isEmptyMarker1)// и до конца строки пишем числа из файла 1
                                        {
                                            writer.Write(number1 + " ");
                                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                        }
                                        writer.Write(number1 + " ");// записываем последнее число з 1 файла которое цикл не покрывает

                                    }
                                }
                                writer.WriteLine();//переходим на новую строку
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else//если числа не последние, просто записываем меньшее и запрашиваем новое
                            {
                                writer.Write(number1 + " ");
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }



                        }
                        else// если число з 2 файла меньше
                        {

                            if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                            {
                                writer.Write(number2 + " ");//вводим меньшее число
                                writer.Write(number1 + " ");// вводим большее число
                                writer.WriteLine();//переходим на новую строку
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                            else if (isEmptyMarker1)// если число из 1 файла последнее в строке
                            {
                                //writer.Write(number2 + " ");
                                while (!isEmptyMarker2)//пока не закончаться числа во 2 файле 
                                {
                                    if (number1 > number2) // если  число из 2 файла меньше, выводим числа из 2 файла
                                    {
                                        writer.Write(number2 + " ");
                                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                        if (isEmptyMarker2 && number1 > number2)//на случай если последнее будет меньше
                                        {
                                            writer.Write(number2 + " ");
                                            writer.Write(number1 + " ");
                                            //writer.WriteLine();
                                        }
                                        else if (isEmptyMarker2 && number1 <= number2)//если последнее будет больше
                                        {
                                            writer.Write(number1 + " ");
                                            writer.Write(number2 + " ");
                                            //writer.WriteLine();
                                        }
                                    }
                                    else //пока они не станут больше
                                    {
                                        writer.Write(number1 + " ");// пишем последнее число з 1 файла
                                        while (!isEmptyMarker2)// и до конца строки пишем числа из файла 2
                                        {
                                            writer.Write(number2 + " ");
                                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                        }
                                        writer.Write(number2 + " ");
                                    }
                                }
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                writer.WriteLine();
                            }
                            else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                            {
                                writer.Write(number2 + " ");//пишем его так как оно меньше
                                while (!isEmptyMarker1)//пишем до конца строки чисел из файла 1
                                {
                                    writer.Write(number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }
                                writer.Write(number1 + " ");//записуем последнее число з 1 файла
                                writer.WriteLine();
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                            else//если числа не последние, просто записываем меньшее и запрашиваем новое
                            {
                                writer.Write(number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                            /*
                            if (isEmptyMarker2)
                            {
                                while (isEmptyMarker1)
                                {
                                    writer.Write(number1);
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }
                                writer.WriteLine();
                            }
                            */

                        }
                    }
                    /*
                                    while (has1)
                                    {
                                        writer.Write(number1 + " ");
                                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                    }

                                    while (has2)
                                    {
                                        writer.Write(number2 + " ");
                                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                    }
                    */
                }


            }
            //если один из файлов закончился, то сливаем оставшиеся числа из другого файла
            int count1 = CountLines(outputfile);//считаем количество строк в файле 3
            RemoveFirstNLines(inputfile1, count1);//удаляем обьедененное колличество слов
            RemoveFirstNLines(inputfile2, count1);
            Console.WriteLine(count1);

            if (!has1 && !has2)
            {
                return;//заканчиваем рекурсию
            }
            else if (!has1)// если первым закончился файл 1
            {
                MergeFiles(inputfile2, outputfile, inputfile1);
            }
            else if (!has2)// если первым закончился файл 2
            {
                MergeFiles(inputfile1, outputfile, inputfile2);
            }




            static bool TryReadNextNumber(StreamReader reader, out int result, out bool isEndOfLine)
            {
                result = 0;
                isEndOfLine = false;
                string currentNumber = "";
                int ch;

                while ((ch = reader.Read()) != -1)
                {
                    char c = (char)ch;
                    if (c == 'E')
                    {
                        result = int.MaxValue;
                        isEndOfLine = true;
                        return true; // "E" обозначает пустой масив чтобы алгоритм сошелся
                    }

                    if (c == '\n')
                    {


                        // Если число накопилось перед \n — сначала его вернём
                        if (currentNumber.Length > 0)
                        {
                            if (int.TryParse(currentNumber, out result))
                            {
                                return true;
                            }
                            else
                            {
                                return false; // ошибка парсинга числа
                            }
                        }

                        // Иначе просто переходим к следующему числу
                        continue;
                    }

                    if (char.IsWhiteSpace(c))
                    {
                        if (currentNumber.Length > 0)
                        {
                            if (int.TryParse(currentNumber, out result))
                            {

                                if (reader.Peek() == '\n' || reader.Peek() == '\r')
                                {
                                    isEndOfLine = true;
                                }
                                return true;
                            }
                            else
                            {
                                return false; // ошибка парсинга числа
                            }
                        }
                        // пробелы до начала числа — игнорируем
                    }
                    else
                    {
                        currentNumber += c;
                    }
                }

                // Дошли до конца файла — возможно, есть ещё одно число
                if (currentNumber.Length > 0)
                {
                    if (int.TryParse(currentNumber, out result))
                    {
                        isEndOfLine = true; // условно считаем, что строка закончилась
                        return true;
                    }
                }

                return false; // действительно конец файла
            }

            static int CountLines(string filePath)//считаем количество строк в файле
            {
                int lineCount = 0;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    int b;
                    while ((b = reader.Read()) != -1)
                    {
                        if (b == '\n')
                            lineCount++;
                    }
                }
                return lineCount;
            }
            static void RemoveFirstNLines(string filePath, int N)
            {
                string tempFilePath = "TEMPFILE.txt";
                using (var input = File.OpenRead(filePath))
                using (var output = File.Create(tempFilePath))
                {
                    int b;
                    int newlineCount = 0;
                    bool startWriting = false;

                    while ((b = input.ReadByte()) != -1)
                    {
                        if (!startWriting)
                        {
                            if (b == '\n')
                            {
                                newlineCount++;
                                if (newlineCount >= N)
                                {
                                    startWriting = true;
                                }
                            }
                            continue;
                        }

                        output.WriteByte((byte)b);
                    }
                }

                // Заменяем оригинальный файл
                File.Delete(filePath);
                File.Move(tempFilePath, filePath);
            }
        }


    }
}
