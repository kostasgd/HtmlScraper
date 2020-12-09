using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Notification.Wpf;
namespace HtmlScraper
{
    public partial class ProjectLoader : Form
    {
        public ProjectLoader()
        {
            InitializeComponent();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            panel2.Width += 5;
            if(panel2.Width >= 700)
            {
                timer1.Stop();
                MainForm mf = new MainForm();
                mf.Show();
                this.Hide();
            }
        }
    }
}
