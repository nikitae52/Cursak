using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cursak
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }  
        public void UpdateProgress(int i)
        {
            progressBar1.Value = i;
            Application.DoEvents(); // Додає оновлення UI
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}
