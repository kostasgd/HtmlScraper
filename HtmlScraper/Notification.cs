using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HtmlScraper
{
    public partial class Notification : Form
    {
        public Notification(String message ,Color bgColor)
        {
            InitializeComponent();
            BackColor = bgColor;
            lbMessage.Text = message;
        }

        private void Notification_Load(object sender, EventArgs e)
        {
            Top = 20;
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);
            timerClose.Start();
        }

        private void Notification_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
