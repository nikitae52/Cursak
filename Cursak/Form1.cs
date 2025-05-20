 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Cursak
{
    public partial class Form1 : Form
    {
        //ініціалізація форми
        private Stopwatch stopwatch = new Stopwatch();

        string fileName;
        //string SortingfileName;
        //string SortedFile3
        private File Sortingfile;
        private File Genfile;
        //private File EnteredFile;
        public Form1()
        {

            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            string subPath = "Kursak_results";

            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
                System.IO.Directory.CreateDirectory(subPath);

        }

        //натискання кнопки створення файлу
        private void button1_Click(object sender, EventArgs e)
        {
            fileName = textBox1.Text.Trim();
            int count, minR, maxR;
            //перевірка чи введене ім'я файлу
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Введіть ім'я файлу!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //перевірка на розмір назви файла
            else if (fileName.Length > 100)
            {
                MessageBox.Show("Назва файлу завелика!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            //введення кількості випадкових чисел у файлі
            string numOfRandElem = textBox2.Text.Trim();

            //перевірка чи взагалі ця кількість введена
            if (string.IsNullOrWhiteSpace(numOfRandElem))
            {
                MessageBox.Show("Введіть кількість чисел!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //перевірка чи введене правильне число
            else if (!int.TryParse(textBox2.Text, out count) || count <= 0 || count > 15000000)
            {
                MessageBox.Show("Введіть коректну кількість чисел!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            //нижня границя генерації
            string minRange = textBox3.Text.Trim();

            //перевірка чи вона введена
            if (string.IsNullOrWhiteSpace(minRange))
            {
                MessageBox.Show("Введіть мінімальне значення діапазону генерації!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //перевірка на порушення дозволеного діапазону
            else if (!int.TryParse(textBox3.Text, out minR) || minR < -1000000)
            {
                MessageBox.Show("Введіть коректне мінімальне значення діапазону генерації!!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            //верхня границя генерації
            string maxRange = textBox4.Text.Trim();
            //перевірка чи вона введена 
            if (string.IsNullOrWhiteSpace(maxRange))
            {
                MessageBox.Show("Введіть максимальне значення діапазону генерації!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //перевірка на порушення дозволеного діапазону
            else if (!int.TryParse(textBox4.Text, out maxR) || maxR > 1000000)
            {
                MessageBox.Show("Введіть коректне максимальне значення діапазону генерації!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //перевірка коректності границь
            if (minR > maxR)
            {
                MessageBox.Show("Мінімальне число не може бути більше максимального!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Genfile = new File(fileName);

            button1.Enabled = false;
            button2.Visible = false;
            Genfile = File.GenerateFile(fileName, count, minR, maxR);
            button1.Enabled = true;
            Sortingfile =null;
            /*
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            label12.Text = "";
            textBox5.Text = "";
            */

            button2.Visible = true;
            button5.Visible = false;
            checkBox4.Checked = false;

            //label13.Text = "";
            
            //fileName = fileName + ".txt";
            /*
            Form2 loadingForm = new Form2();
            loadingForm.Show();

            //випадкова генерація цисел та їх запис до файлу
            Random rand = new Random();
            using (StreamWriter writer = File.CreateText(fileName))
            {
                for (int i = 0; i < count; i++)
                {
                    writer.WriteLine(rand.Next(minR, maxR + 1));

                    loadingForm.UpdateProgress((int)((i * 100.0) / count));

                }
            }

            loadingForm.Close();
            */




            MessageBox.Show($"Файл успішно створений!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }




        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Genfile.FilePath);
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }




        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }



        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                if (Genfile==null)
                {
                    MessageBox.Show("У цій сесії не було створено файлів", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    checkBox4.Checked = false;

                }
                else
                {
                    //SortingfileName = fileName;
                    label8.Visible = false;
                    textBox5.Visible = false;
                    button4.Visible = false;
                    Sortingfile = Genfile;


                }
            }
            else
            {
                Sortingfile = null;
                label8.Visible = true;
                textBox5.Visible = true;
                button4.Visible = true;

            }
        }


        private async void button3_Click(object sender, EventArgs e)
        {
            if (Sortingfile==null)
            {
                MessageBox.Show("Виберіть файл для сортування!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!radioButton1.Checked && !radioButton3.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Виберіть алгоритм!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (radioButton3.Checked)
            {
                string PolyphasePath = "Kursak_results/PolyphaseSort";
                bool exists = System.IO.Directory.Exists(PolyphasePath);
                if (!exists) System.IO.Directory.CreateDirectory(PolyphasePath);

                label12.Text = "00:00:00.010";

                stopwatch.Restart();
                timer1.Start();
                button3.Enabled = false;
                button5.Visible = false;
                label13.Text = "";
                await Task.Run(() =>
                {
                    // Виклик алгоритму
                    Sortingfile.PolyPhaseSort();
                    //(SortedFile3, ReadNumber, WriteNumber) = FileSorter.PolyPhaseSort(SortingfileName);
                });
                button3.Enabled = true;

                timer1.Stop();
                stopwatch.Stop();


                button5.Visible = true;

                label13.Text = "Кількість читань: " + Sortingfile.ReadCount + "\n" + "Кількість записів: " + Sortingfile.WriteCount;

            }
            else if (radioButton2.Checked)
            {
                string ThreeWayMergeSortPath = "Kursak_results/ThreeWayMergeSort";
                bool exists = System.IO.Directory.Exists(ThreeWayMergeSortPath);
                if (!exists) System.IO.Directory.CreateDirectory(ThreeWayMergeSortPath);


                label12.Text = "00:00:00.010";

                stopwatch.Restart();
                timer1.Start();
                button3.Enabled = false;
                button5.Visible = false;
                label13.Text = "";
                await Task.Run(() =>
                {
                    // Виклик алгоритму
                    Sortingfile.ThreeWayMergeSort();
                    
                });

                button3.Enabled = true;
                timer1.Stop();
                stopwatch.Stop();


                button5.Visible = true;

                label13.Text = "Кількість читань: " + Sortingfile.ReadCount + "\n" + "Кількість записів: " + Sortingfile.WriteCount;
            }

            else if (radioButton1.Checked)
            {
                string NaturalSortPath = "Kursak_results/NaturalSort";
                bool exists = System.IO.Directory.Exists(NaturalSortPath);
                if (!exists) System.IO.Directory.CreateDirectory(NaturalSortPath);


                label12.Text = "00:00:00.010";

                stopwatch.Restart();
                timer1.Start();
                button3.Enabled = false;
                button5.Visible = false;
                label13.Text = "";
                await Task.Run(() =>
                {
                    // Виклик алгоритму
                    Sortingfile.NaturalSort();
                });

                button3.Enabled = true;
                timer1.Stop();
                stopwatch.Stop();


                button5.Visible = true;

                label13.Text = "Кількість читань: " + Sortingfile.ReadCount + "\n" + "Кількість записів: " + Sortingfile.WriteCount;

            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void button4_Click(object sender, EventArgs e)
        {
            string EnteredFileName = textBox5.Text.Trim();
            if (string.IsNullOrWhiteSpace(EnteredFileName))
            {
                MessageBox.Show("Введіть шлях до файлу!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!System.IO.File.Exists(EnteredFileName))
            {
                MessageBox.Show("Файл не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (Path.GetExtension(EnteredFileName).ToLower() != ".txt")
            {
                MessageBox.Show("Файл має бути у форматі .txt!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


                Sortingfile = new File(EnteredFileName);
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                label12.Text = "";

                button5.Visible = false;

                label13.Text = "";
                MessageBox.Show("Файл успішно знайдено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);

            
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            label12.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string absolutePath = Path.GetFullPath(Sortingfile.NotEmptyFile);

            if (System.IO.File.Exists(absolutePath))
            {

                Process.Start(absolutePath);
            }

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }


}

