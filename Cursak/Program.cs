using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cursak
{
    public static class Program
    {
        static long ReadCount = 0;
        static long WriteCount = 0;
        public static string NotEmptyFile = "";
       /// <summary>
       /// The main entry point for the application.
       /// </summary>
       [STAThread]
        
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
        public static (string,long,long) PolyPhaseSort(string fileName)
        {
            ReadCount = 0;
            WriteCount = 0;
            string tempfile01 = "Kursak_results/PolyphaseSort/tempfile01.txt"; // Writer 1
            string tempfile02 = "Kursak_results/PolyphaseSort/tempfile02.txt"; // Writer 2
            string tempfile03 = "Kursak_results/PolyphaseSort/tempfile03.txt"; // Getter



            using (StreamReader reader = new StreamReader(fileName))
            using (StreamWriter writer1 = new StreamWriter(tempfile01))
            using (StreamWriter writer2 = new StreamWriter(tempfile02))
            {
                int fib1 = 1, fib2 = 1;
                int activeFib1 = fib1, activeFib2 = fib2;
                int totalWritten1 = 0, totalWritten2 = 0;

                string line = ReadLineCounted(reader);
                int.TryParse(line, out int prevNumb);

                StreamWriter activeWriter = writer1;
                bool writingToFirst = true; // true = writer1, false = writer2

                WriteCounted(activeWriter, prevNumb.ToString());
                while ((line = ReadLineCounted(reader)) != null)
                {
                    if (!int.TryParse(line, out int number)) continue;

                    if (number >= prevNumb)
                    {
                        WriteCounted(activeWriter," "+number.ToString());
                    }
                    else
                    {
                        WriteLineCounted(activeWriter); // закончился подмассив


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
                        WriteCounted(activeWriter, number.ToString());

                    }

                    prevNumb = number;
                }

                WriteLineCounted(activeWriter); // завершить последний подмассив
                if (writingToFirst) totalWritten1++;
                else totalWritten2++;

                // Дописываем недостающие EMPTY
                while (totalWritten1 < activeFib1)
                {
                    WriteLineCounted(writer1,"E");
                    totalWritten1++;
                }

                while (totalWritten2 < activeFib2)
                {
                    WriteLineCounted(writer2, "E");
                    totalWritten2++;
                }

                WriteLineCounted(activeWriter); // Завершить последний подмассив
            }


            MergeFiles(tempfile01, tempfile02, tempfile03);
            return (NotEmptyFile,ReadCount,WriteCount);
            // Console.WriteLine("Подмассивы распределены без накопления в памяти.");
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
                        WriteLineCounted(writer, "E");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                    else if (number1 == int.MaxValue && number2 != int.MaxValue)
                    {
                        while (!isEmptyMarker2)
                        {
                            WriteCounted(writer, number2 + " ");
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }
                        WriteCounted(writer, number2 + " ");
                        WriteLineCounted(writer);
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }
                    else if (number1 != int.MaxValue && number2 == int.MaxValue)
                    {
                        while (!isEmptyMarker1)
                        {
                            WriteCounted(writer, number1 + " ");
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        }
                        WriteCounted(writer, number1 + " ");
                        WriteLineCounted(writer);
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                    else
                    {
                        if (number1 <= number2)
                        {

                            if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                            {
                                WriteCounted(writer, number1 + " ");//вводим меньшеее число з 1 файла
                                WriteCounted(writer, number2 + " ");//вводим большее число з 2 файла
                                WriteLineCounted(writer);//переходим на новую строку
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);

                            }

                            else if (isEmptyMarker1)// если число из 1 файла последнее в строке
                            {
                                WriteCounted(writer, number1 + " ");// записываем его
                                while (!isEmptyMarker2)// пишем числа из файла 2 пока строчка не закончиться
                                {
                                    WriteCounted(writer, number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                }
                                WriteCounted(writer, number2 + " ");// пишем последнее число з 2 файла
                                WriteLineCounted(writer);
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                            {

                                while (!isEmptyMarker1) //пока не закончится строка в 1 файле
                                {
                                    if (number1 <= number2) // если  число из 1 файла меньше, выводим числа из 1 файла
                                    {
                                        WriteCounted(writer, number1 + " ");
                                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);

                                        if (isEmptyMarker1 && number1 <= number2)//если последнее число из 1 файла так и осталось меньше
                                        {
                                            WriteCounted(writer, number1 + " ");
                                            WriteCounted(writer, number2 + " ");
                                            //writer.WriteLine();
                                        }
                                        else if (isEmptyMarker1 && number1 > number2)//если последнее число из 1 файла стало больше
                                        {
                                            WriteCounted(writer, number2 + " ");
                                            WriteCounted(writer, number1 + " ");
                                            //writer.WriteLine();
                                        }
                                    }
                                    else //пока они не станут больше
                                    {
                                        WriteCounted(writer, number2 + " ");// пишем последнее число з 2 файла
                                        while (!isEmptyMarker1)// и до конца строки пишем числа из файла 1
                                        {
                                            WriteCounted(writer, number1 + " ");
                                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                        }
                                        WriteCounted(writer, number1 + " ");// записываем последнее число з 1 файла которое цикл не покрывает

                                    }
                                }
                                WriteLineCounted(writer);//переходим на новую строку
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else//если числа не последние, просто записываем меньшее и запрашиваем новое
                            {
                                WriteCounted(writer, number1 + " ");
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }



                        }
                        else// если число з 2 файла меньше
                        {

                            if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                            {
                                WriteCounted(writer, number2 + " ");//вводим меньшее число
                                WriteCounted(writer, number1 + " ");// вводим большее число
                                WriteLineCounted(writer);//переходим на новую строку
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
                                        WriteCounted(writer, number2 + " ");
                                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                        if (isEmptyMarker2 && number1 > number2)//на случай если последнее будет меньше
                                        {
                                            WriteCounted(writer, number2 + " ");
                                            WriteCounted(writer, number1 + " ");
                                            //writer.WriteLine();
                                        }
                                        else if (isEmptyMarker2 && number1 <= number2)//если последнее будет больше
                                        {
                                            WriteCounted(writer, number1 + " ");
                                            WriteCounted(writer, number2 + " ");
                                            //writer.WriteLine();
                                        }
                                    }
                                    else //пока они не станут больше
                                    {
                                        WriteCounted(writer, number1 + " ");// пишем последнее число з 1 файла
                                        while (!isEmptyMarker2)// и до конца строки пишем числа из файла 2
                                        {
                                            WriteCounted(writer, number2 + " ");
                                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                        }
                                        WriteCounted(writer, number2 + " ");
                                    }
                                }
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                WriteLineCounted(writer);
                            }
                            else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                            {
                                WriteCounted(writer, number2 + " ");//пишем его так как оно меньше
                                while (!isEmptyMarker1)//пишем до конца строки чисел из файла 1
                                {
                                    WriteCounted(writer, number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }
                                WriteCounted(writer, number1 + " ");//записуем последнее число з 1 файла
                                WriteLineCounted(writer);
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                            else//если числа не последние, просто записываем меньшее и запрашиваем новое
                            {
                                WriteCounted(writer, number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                        }
                    }
                }


            }
            //если один из файлов закончился, то сливаем оставшиеся числа из другого файла
            int count1 = CountLines(outputfile);//считаем количество строк в файле 3
            RemoveFirstNLines(inputfile1, count1);//удаляем обьедененное колличество слов
            RemoveFirstNLines(inputfile2, count1);
            //Console.WriteLine(count1);

            if (!has1 && !has2)
            {
                NotEmptyFile = outputfile;
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
 
            
        }

        public static (string, long, long) NaturalSort(string fileName)
        {
 
            ReadCount = 0;
            WriteCount = 0;
            string tempfile01 = "tempfile01.txt"; // Writer 1
            string tempfile02 = "tempfile02.txt"; // Writer 2
            string tempfile03 = "tempfile03.txt"; // Writer 2

            Console.WriteLine("Enter file name:");
            fileName = Console.ReadLine();

            using (StreamReader reader = new StreamReader(fileName))
            using (StreamWriter writer2 = new StreamWriter(tempfile02))
            using (StreamWriter writer3 = new StreamWriter(tempfile03))
            {
                string line = ReadLineCounted(reader);
                int.TryParse(line, out int prevNumb);

                StreamWriter activeWriter = writer2;
                bool writeToSecond = true; // true = writer2, false = writer3

                WriteCounted(activeWriter, prevNumb.ToString());

                while ((line = ReadLineCounted(reader)) != null)
                {
                    if (!int.TryParse(line, out int number)) continue;

                    if (number >= prevNumb)
                    {
                        // продолжаем писать в текущий поток
                        WriteCounted(activeWriter, " " + number.ToString());
                    }
                    else
                    {
                        // Подмассив закончился, переходим на другой поток
                        WriteLineCounted(activeWriter); // Завершаем строку

                        // Меняем поток записи
                        writeToSecond = !writeToSecond;
                        activeWriter = writeToSecond ? writer2 : writer3;

                        // Начинаем новый подмассив
                        WriteCounted(activeWriter, number.ToString());
                    }

                    prevNumb = number;
                }

                // Завершаем последний подмассив
                WriteLineCounted(activeWriter);
            }


            NaturallyMergeFiles(tempfile02, tempfile03, tempfile01);
            return (NotEmptyFile, ReadCount, WriteCount);
            //Console.WriteLine("Подмассивы распределены без накопления в памяти.");
        }

        static void NaturallyMergeFiles(string inputfile1, string inputfile2, string outputfile)
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
                    if (number1 <= number2)
                    {

                        if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                        {
                            WriteCounted(writer, number1 + " ");//вводим меньшеее число з 1 файла
                            WriteCounted(writer, number2 + " ");//вводим большее число з 2 файла
                            WriteLineCounted(writer);//переходим на новую строку
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);

                        }

                        else if (isEmptyMarker1)// если число из 1 файла последнее в строке
                        {
                            WriteCounted(writer, number1 + " ");// записываем его
                            while (!isEmptyMarker2)// пишем числа из файла 2 пока строчка не закончиться
                            {
                                WriteCounted(writer, number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            WriteCounted(writer, number2 + " ");// пишем последнее число з 2 файла
                            WriteLineCounted(writer);
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }
                        else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                        {

                            while (!isEmptyMarker1) //пока не закончится строка в 1 файле
                            {
                                if (number1 <= number2) // если  число из 1 файла меньше, выводим числа из 1 файла
                                {
                                    WriteCounted(writer, number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);

                                    if (isEmptyMarker1 && number1 <= number2)//если последнее число из 1 файла так и осталось меньше
                                    {
                                        WriteCounted(writer, number1 + " ");
                                        WriteCounted(writer, number2 + " ");
                                        //writer.WriteLine();
                                    }
                                    else if (isEmptyMarker1 && number1 > number2)//если последнее число из 1 файла стало больше
                                    {
                                        WriteCounted(writer, number2 + " ");
                                        WriteCounted(writer, number1 + " ");
                                        //writer.WriteLine();
                                    }
                                }
                                else //пока они не станут больше
                                {
                                    WriteCounted(writer, number2 + " ");// пишем последнее число з 2 файла
                                    while (!isEmptyMarker1)// и до конца строки пишем числа из файла 1
                                    {
                                        WriteCounted(writer, number1 + " ");
                                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                    }
                                    WriteCounted(writer, number1 + " ");// записываем последнее число з 1 файла которое цикл не покрывает

                                }
                            }
                            WriteLineCounted(writer);//переходим на новую строку
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//ищем следующие числа
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }
                        else//если числа не последние, просто записываем меньшее и запрашиваем новое
                        {
                            WriteCounted(writer, number1 + " ");
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        }



                    }
                    else// если число з 2 файла меньше
                    {

                        if (isEmptyMarker1 && isEmptyMarker2) //если два числа последние в строке
                        {
                            WriteCounted(writer, number2 + " ");//вводим меньшее число
                            WriteCounted(writer, number1 + " ");// вводим большее число
                            WriteLineCounted(writer);//переходим на новую строку
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
                                    WriteCounted(writer, number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                    if (isEmptyMarker2 && number1 > number2)//на случай если последнее будет меньше
                                    {
                                        WriteCounted(writer, number2 + " ");
                                        WriteCounted(writer, number1 + " ");
                                        //writer.WriteLine();
                                    }
                                    else if (isEmptyMarker2 && number1 <= number2)//если последнее будет больше
                                    {
                                        WriteCounted(writer, number1 + " ");
                                        WriteCounted(writer, number2 + " ");
                                        //writer.WriteLine();
                                    }
                                }
                                else //пока они не станут больше
                                {
                                    WriteCounted(writer, number1 + " ");// пишем последнее число з 1 файла
                                    while (!isEmptyMarker2)// и до конца строки пишем числа из файла 2
                                    {
                                        WriteCounted(writer, number2 + " ");
                                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                    }
                                    WriteCounted(writer, number2 + " ");
                                }
                            }
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            WriteLineCounted(writer);
                        }
                        else if (isEmptyMarker2)//если число из 2 файла последнее в строке
                        {
                            WriteCounted(writer, number2 + " ");//пишем его так как оно меньше
                            while (!isEmptyMarker1)//пишем до конца строки чисел из файла 1
                            {
                                WriteCounted(writer, number1 + " ");
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }
                            WriteCounted(writer, number1 + " ");//записуем последнее число з 1 файла
                            WriteLineCounted(writer);
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }

                        else//если числа не последние, просто записываем меньшее и запрашиваем новое
                        {
                            WriteCounted(writer, number2 + " ");
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }



                    }

                }


            }
            //если один из файлов закончился, то сливаем оставшиеся числа из другого файла
            int count1 = CountLines(outputfile);//считаем количество строк в файле 3
            RemoveFirstNLines(inputfile1, count1);//удаляем обьедененное колличество слов
            RemoveFirstNLines(inputfile2, count1);
            //Console.WriteLine(count1);

            //повторне розбиття за 2 файлами

            if (!has1)// если первым закончился файл 1
            {
                using (StreamReader reader1 = new StreamReader(inputfile1))
                using (StreamReader reader2 = new StreamReader(inputfile2))
                using (StreamWriter writer = new StreamWriter(outputfile, append: true))
                {
                    has2 = TryReadNextNumber(reader2, out int number2, out bool isEmptyMarker2);
                    while (has2)//пока не закончаться числа во 2 файле 
                    {
                        WriteCounted(writer, number2 + " ");
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        if (isEmptyMarker2)//на случай если последнее будет меньше
                        {
                            WriteCounted(writer, number2 + " ");
                            writer.WriteLine();
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);

                        }
                    }

                }
                File.Create(inputfile1).Dispose();
                File.Create(inputfile2).Dispose();
                //NaturallyMergeFiles(inputfile2, outputfile, inputfile1);
            }
            else if (!has2)// если первым закончился файл 2
            {
                using (StreamReader reader1 = new StreamReader(inputfile1))
                using (StreamReader reader2 = new StreamReader(inputfile2))
                using (StreamWriter writer = new StreamWriter(outputfile, append: true))
                {
                    has1 = TryReadNextNumber(reader1, out int number1, out bool isEmptyMarker1);
                    while (has1)//пока не закончаться числа во 2 файле 
                    {
                        WriteCounted(writer, number1 + " ");
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        if (isEmptyMarker1)//на случай если последнее будет меньше
                        {
                            WriteCounted(writer, number1 + " ");
                            writer.WriteLine();
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);

                        }
                    }

                }
                File.Create(inputfile1).Dispose();
                File.Create(inputfile2).Dispose();
                //NaturallyMergeFiles(inputfile1, outputfile, inputfile2);
            }

            using (StreamReader reader = new StreamReader(outputfile))
            {
                bool hass = TryReadNextNumber(reader, out int number0, out bool isEmptyMarker0);
                while (!isEmptyMarker0)
                {
                    hass = TryReadNextNumber(reader, out number0, out isEmptyMarker0);
                }
                hass = TryReadNextNumber(reader, out number0, out isEmptyMarker0);

                if (!hass)
                {
                    return;
                }
                else
                {
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    reader.DiscardBufferedData();
                    using (StreamWriter writer2 = new StreamWriter(inputfile1))
                    using (StreamWriter writer3 = new StreamWriter(inputfile2))
                    {
                        bool has = TryReadNextNumber(reader, out int number, out bool isEmptyMarker);
                        StreamWriter activeWriter = writer2;
                        while (has)
                        {
                            while (!isEmptyMarker)
                            {
                                WriteCounted(activeWriter, number + " ");
                                has = TryReadNextNumber(reader, out number, out isEmptyMarker);
                            }
                            WriteCounted(activeWriter, number.ToString());
                            WriteLineCounted(activeWriter);
                            has = TryReadNextNumber(reader, out number, out isEmptyMarker);
                            if (activeWriter == writer2)
                            {
                                activeWriter = writer3;
                            }
                            else
                            {
                                activeWriter = writer2;
                            }
                        }
                    }

                }
            }
            NaturallyMergeFiles(inputfile1, inputfile2, outputfile);
        }

        static void WriteCounted(StreamWriter writer, string text)
        {
            writer.Write(text);
            WriteCount++;
        }

        static void WriteLineCounted(StreamWriter writer, string text = "")
        {
            writer.WriteLine(text);
            WriteCount++;
        }

        static int ReadCounted(StreamReader reader)
        {
            ReadCount++;
            return reader.Read();
        }

        static string ReadLineCounted(StreamReader reader)
        {
            ReadCount++;
            return reader.ReadLine();
        }

        static int PeekCounted(StreamReader reader)
        {
            ReadCount++;
            return reader.Peek();
        }

        static int ReadByteCounted(FileStream stream)
        {
            ReadCount++;
            return stream.ReadByte();
        }

        static void WriteByteCounted(FileStream stream, byte b)
        {
            WriteCount++;
            stream.WriteByte(b);
        }

        static bool TryReadNextNumber(StreamReader reader, out int result, out bool isEndOfLine)
        {
            result = 0;
            isEndOfLine = false;
            string currentNumber = "";
            int ch;

            while ((ch = ReadCounted(reader)) != -1)
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

                            if (PeekCounted(reader) == '\n' || PeekCounted(reader) == '\r')
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
                while ((b = ReadCounted(reader)) != -1)
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

                while ((b = ReadByteCounted(input)) != -1)
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

                    WriteByteCounted(output, (byte)b);

                }
            }

            // Заменяем оригинальный файл
            File.Delete(filePath);
            File.Move(tempFilePath, filePath);
        }
    }
}


