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
        string SortingfileName;
        string SortedFile3;
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
            int count,minR,maxR;
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

            fileName = fileName + ".txt";


            //введення кількості випадкових чисел у файлі
            string numOfRandElem = textBox2.Text.Trim();

            //перевірка чи взагалі ця кількість введена
            if (string.IsNullOrWhiteSpace(numOfRandElem))
            {
                MessageBox.Show("Введіть кількість чисел!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //перевірка чи введене правильне число
            else if (!int.TryParse(textBox2.Text, out count) || count <= 0 || count >5000000)
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
            else if (!int.TryParse(textBox3.Text, out minR) || minR  <-1000000)
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
            else if (!int.TryParse(textBox4.Text, out maxR) || maxR>1000000)
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


            Form2 loadingForm = new Form2();
            loadingForm.Show();

            //випадкова генерація цисел та їх запис до файлу
            Random rand = new Random();
            using (StreamWriter writer = File.CreateText(fileName))
            {
                for (int i = 0; i < count; i++)
                {
                    writer.WriteLine(rand.Next(minR, maxR+1));

                    loadingForm.UpdateProgress((int)((i * 100.0) / count));

                }
            }

            loadingForm.Close();

            button2.Visible = true;


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
            Process.Start("explorer.exe", fileName);
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
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    MessageBox.Show("У цій сесії не було створено файлів", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    checkBox4.Checked = false;

                }
                else
                {
                    SortingfileName = fileName;
                    label8.Visible = false;
                    textBox5.Visible = false;
                    button4.Visible = false;

                }
            }
            else
            {
                SortingfileName = null;
                label8.Visible = true;
                textBox5.Visible = true;
                button4.Visible = true;

            }
        }


        private async void button3_Click(object sender, EventArgs e)
        {
            if (SortingfileName == null)
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

                long ReadNumber = 0;
                long WriteNumber = 0;

                label12.Text = "00:00:00.010";

                stopwatch.Restart();
                timer1.Start();

                await Task.Run(() =>
                {
                    // Виклик алгоритму
                    (SortedFile3,ReadNumber,WriteNumber) = Program.PolyPhaseSort(SortingfileName);
                });


                timer1.Stop();
                stopwatch.Stop();


                button5.Visible = true;

                label13.Text = "Кількість читань: " + ReadNumber+"\n"+"Кількість записів: " + WriteNumber;

            }
            else if (radioButton2.Checked)
            {
                string ThreeWayMergeSortPath = "Kursak_results/ThreeWayMergeSort";
                bool exists = System.IO.Directory.Exists(ThreeWayMergeSortPath);
                if (!exists) System.IO.Directory.CreateDirectory(ThreeWayMergeSortPath);

                long ReadNumber = 0;
                long WriteNumber = 0;

                label12.Text = "00:00:00.010";

                stopwatch.Restart();
                timer1.Start();

                await Task.Run(() =>
                {
                    // Виклик алгоритму
                    (SortedFile3, ReadNumber, WriteNumber) = Program.ThreeWayMergeFiles(SortingfileName);
                });


                timer1.Stop();
                stopwatch.Stop();


                button5.Visible = true;

                label13.Text = "Кількість читань: " + ReadNumber + "\n" + "Кількість записів: " + WriteNumber;
            }

            else if (radioButton1.Checked)
            {
                string NaturalSortPath = "Kursak_results/NaturalSort";
                bool exists = System.IO.Directory.Exists(NaturalSortPath);
                if (!exists) System.IO.Directory.CreateDirectory(NaturalSortPath);

                long ReadNumber = 0;
                long WriteNumber = 0;

                label12.Text = "00:00:00.010";

                stopwatch.Restart();
                timer1.Start();

                await Task.Run(() =>
                {
                    // Виклик алгоритму
                    (SortedFile3, ReadNumber, WriteNumber) = Program.NaturalSort(SortingfileName);
                });


                timer1.Stop();
                stopwatch.Stop();


                button5.Visible = true;

                label13.Text = "Кількість читань: " + ReadNumber + "\n" + "Кількість записів: " + WriteNumber;

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
            else if (!File.Exists(EnteredFileName))
            {
                MessageBox.Show("Файл не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (Path.GetExtension(EnteredFileName).ToLower() != ".txt")
            {
                MessageBox.Show("Файл має бути у форматі .txt!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var reader = new StreamReader(EnteredFileName))
            {

                    SortingfileName = EnteredFileName;
                    MessageBox.Show("Файл успішно знайдено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
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
            string absolutePath = Path.GetFullPath(SortedFile3);

            if (File.Exists(absolutePath))
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
    }
}
