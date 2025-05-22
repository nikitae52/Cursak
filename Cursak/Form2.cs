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
    public partial class LoadingForm : Form
    {
        public LoadingForm()
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
