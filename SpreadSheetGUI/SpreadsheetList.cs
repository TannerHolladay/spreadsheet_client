using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadSheetGUI
{
    public partial class SpreadsheetList : Form
    {
        private string[] spreadsheetNames;

        public string sspreadSheetName = "";
        public SpreadsheetList()
        {
            InitializeComponent();
        }

        public void SetSpreadsheetNames(string[] spreadsheetNames)
        {
            this.spreadsheetNames = spreadsheetNames;
            for (int i = 0; i < spreadsheetNames.Length - 2; i++)
            {
                NamesListBox.Items.Add(spreadsheetNames[i]);
            }
        }

        private void NamesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedNameTextBox.Text = NamesListBox.SelectedItem.ToString();
        }

       private void SendNameButton_Click(object sender, EventArgs e)
        {
            sspreadSheetName = SelectedNameTextBox.Text;
            this.Close();
        
        }
    }
}
