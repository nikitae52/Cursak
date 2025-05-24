using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursak
{
    public class File
    {
        public long ReadCount=0;
        public long WriteCount=0;
        public string NotEmptyFile = "";

        public string FilePath { get; set; }

        public File(string fileName)
        {
            FilePath = fileName;
        }
        public static File GenerateFile(string fileName, int count, int minR, int maxR)
        {
            LoadingForm loadingForm = new LoadingForm();
            loadingForm.Show();

            string path = fileName + ".txt";
            Random rand = new Random();

            using (StreamWriter writer = new StreamWriter(path))
            {
                for (int i = 0; i < count; i++)
                {
                    writer.WriteLine(rand.Next(minR, maxR + 1));
                    loadingForm.UpdateProgress((int)((i * 100.0) / count));
                }
            }

            loadingForm.Close();
            return new File(path);
        }
        public void PolyPhaseSort()
        {
            ReadCount = 0;
            WriteCount = 0;
            string tempfile01 = "Kursak_results/PolyphaseSort/tempfile01.txt"; // Writer 1
            string tempfile02 = "Kursak_results/PolyphaseSort/tempfile02.txt"; // Writer 2
            string tempfile03 = "Kursak_results/PolyphaseSort/tempfile03.txt"; // Getter

            string fileName = this.FilePath;

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
                        WriteCounted(activeWriter, " " + number.ToString());
                    }
                    else
                    {
                        WriteLineCounted(activeWriter); // закінчився підмассив


                        // підрахунок підмассивів
                        if (writingToFirst) totalWritten1++;
                        else totalWritten2++;

                        // перевірка на досягнення ліміту
                        if (totalWritten1 >= activeFib1 && totalWritten2 >= activeFib2)
                        {
                            int next = fib1 + fib2;
                            fib1 = fib2;
                            fib2 = next;
                            activeFib1 = fib1;
                            activeFib2 = fib2;

                        }

                        // переключаємось на потрібний потік
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

                        // починаємо записувати новий підмассив
                        WriteCounted(activeWriter, number.ToString());

                    }

                    prevNumb = number;
                }

                WriteLineCounted(activeWriter); // закінчити написання останнього підмасиву
                if (writingToFirst) totalWritten1++;
                else totalWritten2++;

                // дописуємо недостатні EMPTY
                while (totalWritten1 < activeFib1)
                {
                    WriteLineCounted(writer1, "E");
                    totalWritten1++;
                }

                while (totalWritten2 < activeFib2)
                {
                    WriteLineCounted(writer2, "E");
                    totalWritten2++;
                }

                WriteLineCounted(activeWriter); // Завершити останній підмассив
            }


            MergeFiles(tempfile01, tempfile02, tempfile03);
            //return (NotEmptyFile, ReadCount, WriteCount);
            // Console.WriteLine("Подмассивы распределены без накопления в памяти.");
        }

        public void NaturalSort()
        {
            string fileName = this.FilePath;
            ReadCount = 0;
            WriteCount = 0;
            string tempfile01 = "Kursak_results/NaturalSort/tempfile01.txt"; // Writer 1
            string tempfile02 = "Kursak_results/NaturalSort/tempfile02.txt"; // Writer 2
            string tempfile03 = "Kursak_results/NaturalSort/tempfile03.txt"; // Writer 2

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
                        // продовжуємо писати в поточний потік
                        WriteCounted(activeWriter, " " + number.ToString());
                    }
                    else
                    {
                        // підмассив закінчився, переходимо на інший потік
                        WriteLineCounted(activeWriter); // завершуємо рядок

                        // змінюємо потік запису
                        writeToSecond = !writeToSecond;
                        activeWriter = writeToSecond ? writer2 : writer3;

                        // починаємо новий підмассив
                        WriteCounted(activeWriter, number.ToString());
                    }

                    prevNumb = number;
                }

                // завершуємо останній підмассив
                WriteLineCounted(activeWriter);
            }


            NaturallyMergeFiles(tempfile02, tempfile03, tempfile01);
            NotEmptyFile = tempfile01;
            //return (tempfile01, ReadCount, WriteCount);
            //Console.WriteLine("Подмассивы распределены без накопления в памяти.");

        }

        public void ThreeWayMergeSort()
        {
            string fileName = this.FilePath;
            ReadCount = 0;
            WriteCount = 0;
            string tempfileB1 = "Kursak_results/ThreeWayMergeSort/B01.txt"; // Writer 1
            string tempfileB2 = "Kursak_results/ThreeWayMergeSort/B02.txt"; // Writer 2
            string tempfileB3 = "Kursak_results/ThreeWayMergeSort/B03.txt"; // Writer 2

            using (StreamReader reader = new StreamReader(fileName))
            using (StreamWriter writer1 = new StreamWriter(tempfileB1))
            using (StreamWriter writer2 = new StreamWriter(tempfileB2))
            using (StreamWriter writer3 = new StreamWriter(tempfileB3))
            {
                string line = ReadLineCounted(reader);
                int.TryParse(line, out int prevNumb);

                StreamWriter activeWriter = writer1;
                //bool writeToSecond = true; // true = writer2, false = writer3

                WriteCounted(activeWriter, prevNumb.ToString());

                while ((line = ReadLineCounted(reader)) != null)
                {
                    if (!int.TryParse(line, out int number)) continue;

                    if (number >= prevNumb)
                    {
                        // продовжуємо писати в поточний потік
                        WriteCounted(activeWriter, " " + number.ToString());
                    }
                    else
                    {
                        // підмассив закінчився, переходимо на інший потік
                        WriteLineCounted(activeWriter); // завершуємо рядок

                        // змінюємо потік запису
                        if (activeWriter == writer1)
                        {
                            activeWriter = writer2;
                        }
                        else if (activeWriter == writer2)
                        {
                            activeWriter = writer3;
                        }
                        else
                        {
                            activeWriter = writer1;
                        }

                        // починаємо новий підмассив
                        WriteCounted(activeWriter, number.ToString());
                    }

                    prevNumb = number;
                }

                // завершуємо останній підмассив
                WriteLineCounted(activeWriter);
            }


            NotEmptyFile = MergeFilesINThreeWay(tempfileB1, tempfileB2, tempfileB3);
            //return (resultfile, ReadCount, WriteCount);
            //Console.WriteLine($"Подмассивы распределены без накопления в памяти, result is in,{resultfile}");
        }
        void MergeFiles(string inputfile1, string inputfile2, string outputfile)
        {
            bool has1, has2;//оголошуємо тут щоб потім аналізувати для рекурсії
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

                            if (isEmptyMarker1 && isEmptyMarker2) //якщо два числа останні в строке
                            {
                                WriteCounted(writer, number1 + " ");//вводимо меньше число з 1 файла
                                WriteCounted(writer, number2 + " ");//вводимо більше число з 2 файла
                                WriteLineCounted(writer);//переходимо на новий рядок
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//шукаємо наступні числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);

                            }

                            else if (isEmptyMarker1)// якщо число з 1 файла останнє в рядку
                            {
                                WriteCounted(writer, number1 + " ");// ззаписуємо його
                                while (!isEmptyMarker2)// пишемо числа з файла 2 поки рядок не закінчиться
                                {
                                    WriteCounted(writer, number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                }
                                WriteCounted(writer, number2 + " ");// пишемо останнє число з 2 файла
                                WriteLineCounted(writer);
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//шукаємо наступні числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else if (isEmptyMarker2)//якщо число з 2 файла останнє в рядку
                            {

                                while (!isEmptyMarker1) //поки не закінчиться рядок в 1 файлі
                                {
                                    if (number1 <= number2) // якщо число з 1 файла менше, виводимо числа з 1 файла
                                    {
                                        WriteCounted(writer, number1 + " ");
                                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);

                                        if (isEmptyMarker1 && number1 <= number2)//якщо останнє число з 1 файла так і залишилось менше
                                        {
                                            WriteCounted(writer, number1 + " ");
                                            WriteCounted(writer, number2 + " ");
                                            //writer.WriteLine();
                                        }
                                        else if (isEmptyMarker1 && number1 > number2)//якщо останнє число з 1 файлу стало більше
                                        {
                                            WriteCounted(writer, number2 + " ");
                                            WriteCounted(writer, number1 + " ");
                                            //writer.WriteLine();
                                        }
                                    }
                                    else //поки вони не стануть більше
                                    {
                                        WriteCounted(writer, number2 + " ");// пишемо останнє число з 2 файла
                                        while (!isEmptyMarker1)// і до кінця рядка пишемо число з файлу 1
                                        {
                                            WriteCounted(writer, number1 + " ");
                                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                        }
                                        WriteCounted(writer, number1 + " ");// записуємо останнє число з 1 файла яке цикл не покриває

                                    }
                                }
                                WriteLineCounted(writer);//переходимо на новий рядок
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//шукаємо наступні числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            else//якщо числа не останні, просто записуємо менше і запитуємо нове
                            {
                                WriteCounted(writer, number1 + " ");
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }



                        }
                        else// якщо число з 2 файла менше
                        {

                            if (isEmptyMarker1 && isEmptyMarker2) //якщо два числа останні в ряжку
                            {
                                WriteCounted(writer, number2 + " ");//вводимо менше число
                                WriteCounted(writer, number1 + " ");// вводимо більше число
                                WriteLineCounted(writer);//переходимо на новий рядок
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//шукаємо наступні числа
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                            else if (isEmptyMarker1)// якщо число з 1 файла останнє в рядку
                            {
                                //writer.Write(number2 + " ");
                                while (!isEmptyMarker2)//поки не закінчаться числа в 2 файлі
                                {
                                    if (number1 > number2) // якщо число з 2 файлу менше, виводимо числа з 2 файлу
                                    {
                                        WriteCounted(writer, number2 + " ");
                                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                        if (isEmptyMarker2 && number1 > number2)//на випадок якщо останнє буде менше
                                        {
                                            WriteCounted(writer, number2 + " ");
                                            WriteCounted(writer, number1 + " ");
                                            //writer.WriteLine();
                                        }
                                        else if (isEmptyMarker2 && number1 <= number2)//если останнє буде більше
                                        {
                                            WriteCounted(writer, number1 + " ");
                                            WriteCounted(writer, number2 + " ");
                                            //writer.WriteLine();
                                        }
                                    }
                                    else //поки вони не стануть більше
                                    {
                                        WriteCounted(writer, number1 + " ");// пишемо останнє число з 1 файла
                                        while (!isEmptyMarker2)// і до кінця рядка пишемо числа з файлу 2
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
                            else if (isEmptyMarker2)//якщо число з 2 файла останнє в рядку
                            {
                                WriteCounted(writer, number2 + " ");//пишемо його так як воно менше
                                while (!isEmptyMarker1)//пишемо до кінця рядка числа з файлу 1
                                {
                                    WriteCounted(writer, number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                }
                                WriteCounted(writer, number1 + " ");//записуємо останнє число з 1 файла
                                WriteLineCounted(writer);
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                            else//якщо числа не останні, просто записуємо менше і запитуємо нове
                            {
                                WriteCounted(writer, number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }

                        }
                    }
                }


            }
            //якщо один з файлів закінчився, то зливаємо залишкові числа з іншого файлу
            int count1 = CountLines(outputfile);//зчитаємо кількість рядків в файлі 3
            RemoveFirstNLines(inputfile1, count1);//видаляємо об'єднану кількість рядків
            RemoveFirstNLines(inputfile2, count1);
            //Console.WriteLine(count1);

            if (!has1 && !has2)
            {
                NotEmptyFile = outputfile;
                return;//завершуємо рекурсію
            }
            else if (!has1)// якщо перший закінчився файл 1
            {
                MergeFiles(inputfile2, outputfile, inputfile1);
            }
            else if (!has2)// якщо першим закінчився файл 2
            {
                MergeFiles(inputfile1, outputfile, inputfile2);
            }


        }

        void NaturallyMergeFiles(string inputfile1, string inputfile2, string outputfile)
        {
            bool has1, has2;//оголошуємо тут щоб потім аналізувати для рекурсії
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

                        if (isEmptyMarker1 && isEmptyMarker2) //якщо два числа останні в рядку
                        {
                            WriteCounted(writer, number1 + " ");//вводимо менше число з 1 файла
                            WriteCounted(writer, number2 + " ");//вводимо більше число з 2 файла
                            WriteLineCounted(writer);//переходимо на новий рядок
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//шукаємо наступне число
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);

                        }

                        else if (isEmptyMarker1)// якщо число з 1 файла останнє в рядку
                        {
                            WriteCounted(writer, number1 + " ");// записуємо його
                            while (!isEmptyMarker2)// пишемо числа з файла 2 поки рядок не закінчиться
                            {
                                WriteCounted(writer, number2 + " ");
                                has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                            }
                            WriteCounted(writer, number2 + " ");// пишемо останнє число з 2 файла
                            WriteLineCounted(writer);
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//шукаємо наступні числа
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }
                        else if (isEmptyMarker2)//шукаємо число з 2 файла останнє в рядку
                        {

                            while (!isEmptyMarker1) //поки не закінчиться рядок в 1 файлі
                            {
                                if (number1 <= number2) // якщо число з 1 файла менше, виводимо числа з 1 файла
                                {
                                    WriteCounted(writer, number1 + " ");
                                    has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);

                                    if (isEmptyMarker1 && number1 <= number2)//якщо останнє число  з 1 файла так і залишилось менше
                                    {
                                        WriteCounted(writer, number1 + " ");
                                        WriteCounted(writer, number2 + " ");
                                        //writer.WriteLine();
                                    }
                                    else if (isEmptyMarker1 && number1 > number2)//якщо останнє число з 1 файла стало більше
                                    {
                                        WriteCounted(writer, number2 + " ");
                                        WriteCounted(writer, number1 + " ");
                                        //writer.WriteLine();
                                    }
                                }
                                else //поки вони не стануть більше
                                {
                                    WriteCounted(writer, number2 + " ");// пишемо останнє число з 2 файла
                                    while (!isEmptyMarker1)// і до кінця рядка пишемо число з файлу 1
                                    {
                                        WriteCounted(writer, number1 + " ");
                                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                                    }
                                    WriteCounted(writer, number1 + " ");// записуємо останнє число з 1 файла яке цикл не покриває

                                }
                            }
                            WriteLineCounted(writer);//переходимо на новий рядок
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//шукаємо наступні числа
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }
                        else//якщо числа не останні, просто записуємо менше і запитуємо нове
                        {
                            WriteCounted(writer, number1 + " ");
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        }



                    }
                    else// якщо число з 2 файла менше
                    {

                        if (isEmptyMarker1 && isEmptyMarker2) //якщо два числа останні в рядку
                        {
                            WriteCounted(writer, number2 + " ");//вводимо менше число
                            WriteCounted(writer, number1 + " ");// вводимо більше число
                            WriteLineCounted(writer);//переходимо на новий рядок
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);//шукаємо наступне число
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }

                        else if (isEmptyMarker1)// якщо число з 1 файла останнє в рядку
                        {
                            //writer.Write(number2 + " ");
                            while (!isEmptyMarker2)//поки не закінчаться числа в 2 файлі
                            {
                                if (number1 > number2) // якщо число з 2 файлу менше, виводимо числа з 2 файлу
                                {
                                    WriteCounted(writer, number2 + " ");
                                    has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                                    if (isEmptyMarker2 && number1 > number2)//на випадок якщо останнє буде менше
                                    {
                                        WriteCounted(writer, number2 + " ");
                                        WriteCounted(writer, number1 + " ");
                                        //writer.WriteLine();
                                    }
                                    else if (isEmptyMarker2 && number1 <= number2)//якщо останнє буде більше
                                    {
                                        WriteCounted(writer, number1 + " ");
                                        WriteCounted(writer, number2 + " ");
                                        //writer.WriteLine();
                                    }
                                }
                                else //поки вони не стануть більше
                                {
                                    WriteCounted(writer, number1 + " ");// пишемо остання число з 1 файла
                                    while (!isEmptyMarker2)// і до кінця рядка пишемо числа з файлу 2
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
                        else if (isEmptyMarker2)//якщо число з 2 файла останнє в рядку
                        {
                            WriteCounted(writer, number2 + " ");//пишемо його так як воно менше
                            while (!isEmptyMarker1)//пишемо до кінця радка числа з файлу 1
                            {
                                WriteCounted(writer, number1 + " ");
                                has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            }
                            WriteCounted(writer, number1 + " ");//записуемо останнє число з 1 файла
                            WriteLineCounted(writer);
                            has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }

                        else//якщо числа не останні, просто записуємо менше і запитуємо нове
                        {
                            WriteCounted(writer, number2 + " ");
                            has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        }



                    }

                }


            }
            //якщо один з файлів закінчився, то зливаємо залишкові числа з іншого файлу
            int count1 = CountLines(outputfile);//рахуємо кількість рядків в файлі 3
            RemoveFirstNLines(inputfile1, count1);//видаляємо об'єднану кількість рядків
            RemoveFirstNLines(inputfile2, count1);
            //Console.WriteLine(count1);

            //повторне розбиття за 2 файлами

            if (!has1)// якщо першим закінчився файл 1
            {
                using (StreamReader reader1 = new StreamReader(inputfile1))
                using (StreamReader reader2 = new StreamReader(inputfile2))
                using (StreamWriter writer = new StreamWriter(outputfile, append: true))
                {
                    has2 = TryReadNextNumber(reader2, out int number2, out bool isEmptyMarker2);
                    while (has2)//поки не закінчаться числа в 2 файлі
                    {
                        WriteCounted(writer, number2 + " ");
                        if (isEmptyMarker2)//на випад якщо останнє буде менше
                        {

                            writer.WriteLine();
                        }
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    }

                }
                System.IO.File.Create(inputfile1).Dispose();
                System.IO.File.Create(inputfile2).Dispose();
                //NaturallyMergeFiles(inputfile2, outputfile, inputfile1);
            }
            else if (!has2)// якщо першим закінчився файл 2
            {
                using (StreamReader reader1 = new StreamReader(inputfile1))
                using (StreamReader reader2 = new StreamReader(inputfile2))
                using (StreamWriter writer = new StreamWriter(outputfile, append: true)) // записуємо в кінець файлу не переписуючи його
                {
                    has1 = TryReadNextNumber(reader1, out int number1, out bool isEmptyMarker1);
                    while (has1)//поки не закінчаться числа в 1 файлі
                    {
                        WriteCounted(writer, number1 + " ");
                        if (isEmptyMarker1)//на випадок якщо останнє буде менше
                        {
                            writer.WriteLine();
                        }
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                    }

                }
                System.IO.File.Create(inputfile1).Dispose();
                System.IO.File.Create(inputfile2).Dispose();
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

        string MergeFilesINThreeWay(string inputfile1, string inputfile2, string inputfile3)
        {
            bool has1, has2, has3; // 3 файла
            bool line1finished = false;
            bool line2finished = false;
            bool line3finished = false;
            string outputfile1;
            string outputfile2;
            string outputfile3;
            string tempfileB1 = "Kursak_results/ThreeWayMergeSort/B01.txt";
            string tempfileB2 = "Kursak_results/ThreeWayMergeSort/B02.txt";
            string tempfileB3 = "Kursak_results/ThreeWayMergeSort/B03.txt";
            string tempfileC1 = "Kursak_results/ThreeWayMergeSort/C01.txt"; // Writer 2
            string tempfileC2 = "Kursak_results/ThreeWayMergeSort/C02.txt"; // Writer 2
            string tempfileC3 = "Kursak_results/ThreeWayMergeSort/C03.txt"; // Writer 2

            if (inputfile1 == tempfileB1)
            {
                outputfile1 = tempfileC1;
                outputfile2 = tempfileC2;
                outputfile3 = tempfileC3;
            }
            else
            {
                outputfile1 = tempfileB1;
                outputfile2 = tempfileB2;
                outputfile3 = tempfileB3;
            }

            using (StreamReader reader1 = new StreamReader(inputfile1))
            using (StreamReader reader2 = new StreamReader(inputfile2))
            using (StreamReader reader3 = new StreamReader(inputfile3))
            using (StreamWriter writer1 = new StreamWriter(outputfile1))
            using (StreamWriter writer2 = new StreamWriter(outputfile2))
            using (StreamWriter writer3 = new StreamWriter(outputfile3))
            {
                has1 = TryReadNextNumber(reader1, out int number1, out bool isEmptyMarker1);
                if (!has1) line1finished = true;
                has2 = TryReadNextNumber(reader2, out int number2, out bool isEmptyMarker2);
                if (!has2) line2finished = true;
                has3 = TryReadNextNumber(reader3, out int number3, out bool isEmptyMarker3);
                if (!has3) line3finished = true;
                StreamWriter ActiveWriter = writer1;

                while (has1 || has2 || has3)
                {
                    int minValue = int.MinValue;
                    int fromWhich = 0;

                    if (line1finished && line2finished && line3finished)
                    {
                        line1finished = false;
                        line2finished = false;
                        line3finished = false;
                        WriteLineCounted(ActiveWriter);
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                        if (!has1) line1finished = true;
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                        if (!has2) line2finished = true;
                        has3 = TryReadNextNumber(reader3, out number3, out isEmptyMarker3);
                        if (!has3) line3finished = true;

                        if (line1finished && line2finished && line3finished) continue;
                        if (ActiveWriter == writer1)
                        {
                            ActiveWriter = writer2;
                        }
                        else if (ActiveWriter == writer2)
                        {
                            ActiveWriter = writer3;
                        }
                        else
                        {
                            ActiveWriter = writer1;
                        }
                    }

                    if (has1 && (!has2 || number1 <= number2) && (!has3 || number1 <= number3) && !line1finished)
                    {
                        minValue = number1;
                        fromWhich = 1;
                        if (isEmptyMarker1 == true)
                        {
                            line1finished = true;
                            number1 = int.MaxValue;
                        }

                    }
                    else if (has2 && (!has1 || number2 <= number1) && (!has3 || number2 <= number3) && !line2finished)
                    {
                        minValue = number2;
                        fromWhich = 2;
                        if (isEmptyMarker2 == true)
                        {
                            line2finished = true;
                            number2 = int.MaxValue;
                        }
                    }
                    else if (!line3finished)// has3
                    {
                        minValue = number3;
                        fromWhich = 3;
                        if (isEmptyMarker3 == true)
                        {
                            line3finished = true;
                            number3 = int.MaxValue;
                        }
                    }

                    // Пишемо найменше число в активний файл
                    WriteCounted(ActiveWriter, minValue + " ");


                    // оновляємо число з того файлу, звідки ми взяли найменше
                    if (fromWhich == 1 && !line1finished)
                        has1 = TryReadNextNumber(reader1, out number1, out isEmptyMarker1);
                    else if (fromWhich == 2 && !line2finished)
                        has2 = TryReadNextNumber(reader2, out number2, out isEmptyMarker2);
                    else if (fromWhich == 3 && !line3finished)
                        has3 = TryReadNextNumber(reader3, out number3, out isEmptyMarker3);
                }

            }
            using (StreamReader writer1 = new StreamReader(outputfile1))
            using (StreamReader writer2 = new StreamReader(outputfile2))
            using (StreamReader writer3 = new StreamReader(outputfile3))
            {
                has1 = TryReadNextNumber(writer1, out int number1, out bool isEmptyMarker1);
                has2 = TryReadNextNumber(writer2, out int number2, out bool isEmptyMarker2);
                has3 = TryReadNextNumber(writer3, out int number3, out bool isEmptyMarker3);
                if ((has1 && !has2 && !has3))
                {
                    return outputfile1;
                }
                else if (!has1 && has2 && !has3)
                {
                    return outputfile2;
                }
                else if (!has1 && !has2 && has3)
                {
                    return outputfile3;
                }
            }
            return MergeFilesINThreeWay(outputfile1, outputfile2, outputfile3);
        }
        void WriteCounted(StreamWriter writer, string text)
        {
            writer.Write(text);
            WriteCount++;
        }

        void WriteLineCounted(StreamWriter writer, string text = "")
        {
            writer.WriteLine(text);
            WriteCount++;
        }

        int ReadCounted(StreamReader reader)
        {
            ReadCount++;
            return reader.Read();
        }

        string ReadLineCounted(StreamReader reader)
        {
            ReadCount++;
            return reader.ReadLine();
        }

        int PeekCounted(StreamReader reader)
        {
            ReadCount++;
            return reader.Peek();
        }

        int ReadByteCounted(FileStream stream)
        {
            ReadCount++;
            return stream.ReadByte();
        }

        void WriteByteCounted(FileStream stream, byte b)
        {
            WriteCount++;
            stream.WriteByte(b);
        }

        bool TryReadNextNumber(StreamReader reader, out int result, out bool isEndOfLine)
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
                    return true; // "E" ознчає порожній масив, необхідний для сходження алгоритму
                }

                if (c == '\n')
                {


                    // Якщо накопичилось число перед \n — спочатку його повернемо
                    if (currentNumber.Length > 0)
                    {
                        if (int.TryParse(currentNumber, out result))
                        {
                            return true;
                        }
                        else
                        {
                            return false; // помилка парсингу числа
                        }
                    }

                    // іначе просто переходимо до наступного числа
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
                            return false;  // помилка парсингу числа
                        }
                    }
                    //ігноруємо пробіли до початку числа
                }
                else
                {
                    currentNumber += c;
                }
            }

            //Дійшли до кінця строки - можливо, є ще одне число
            if (currentNumber.Length > 0)
            {
                if (int.TryParse(currentNumber, out result))
                {
                    isEndOfLine = true; // умовно вважаємо, що це кінець рядка
                    return true;
                }
            }

            return false; // дійсно кінець файлу
        }

        int CountLines(string filePath)//рахуємо кількість рядків у файлі
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

        void RemoveFirstNLines(string filePath, int N)
        {
            if (N <= 0) return;
            string tempFilePath = "TEMPFILE.txt";
            using (var input = System.IO.File.OpenRead(filePath))
            using (var output = System.IO.File.Create(tempFilePath))
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

            // замінюємо оригінальний файл на тимчасовий
            System.IO.File.Delete(filePath);
            System.IO.File.Move(tempFilePath, filePath);
        }
    }
}
