using System;
using System.Windows.Forms;

namespace SpreadSheetGUI
{
    internal partial class HelpBox : Form
    {
        public HelpBox()
        {
            InitializeComponent();
            richTextBox1.SelectAll();
            richTextBox1.SelectionBullet = true;
            richTextBox1.Select(0, 0);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}