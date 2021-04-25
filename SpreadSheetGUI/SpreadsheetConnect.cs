using System;
using System.Windows.Forms;
using Control;

namespace SpreadSheetGUI
{
    public partial class SpreadsheetConnect : Form
    {
        private string _currentName;
        private string _currentIP;

        private Controller _clientController;

        public SpreadsheetConnect()
        {
            InitializeComponent();

            _clientController = new Controller();

            _clientController.GetSpreadsheets += SetSpreadsheetNames;
            _clientController.Error += (message, title) =>
            {
                SpreadsheetForm.Warning(message, title, SpreadsheetForm.WarningType.Error);
            };
            _clientController.Connected += () =>
            {
                _currentName = Username.Text;
                _currentIP = IpAddress.Text;
                ButtonConnect.Enabled = false;
            };
            _clientController.Disconnected += () =>
            {
                _currentName = null;
                _currentIP = null;
                ButtonConnect.Enabled = true;
            };
            Username.TextChanged += Connection_TextChanged;
            IpAddress.TextChanged += Connection_TextChanged;
        }

        /// <summary>
        /// Called when spreadsheet names are received from server.
        /// Displays spreadsheet names on a textbox. 
        /// </summary>
        /// <param name="spreadsheetNames"></param>
        public void SetSpreadsheetNames(string[] spreadsheetNames)
        {
            SpreadsheetList.Items.Clear();
            if (spreadsheetNames.Length > 0)
            {
                foreach (string sheet in spreadsheetNames)
                {
                    SpreadsheetList.Items.Add(sheet);
                }
            }
            else
            {
                SpreadsheetList.Enabled = false;
                SpreadsheetList.Items.Add("No spreadsheets have been created. Please create a new one.");
            }
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            string username = Username.Text;
            if (string.IsNullOrWhiteSpace(username))
            {
                SpreadsheetForm.Warning("Error: Please enter a username", "Empty Username Error",
                    SpreadsheetForm.WarningType.Error);
            }

            string address = IpAddress.Text;
            if (string.IsNullOrWhiteSpace(address))
            {
                SpreadsheetForm.Warning("Error: Please enter an IP Address", "Empty IP Address Error",
                    SpreadsheetForm.WarningType.Error);
            }

            _clientController.Connect(username, address);
        }

        private void Join_Click(object sender, EventArgs e)
        {
            _clientController.SendSpreadsheetRequest(SpreadsheetName.Text);
            Hide();
            SpreadsheetForm form = new SpreadsheetForm(_clientController, SpreadsheetName.Text);
            form.ShowDialog();
            Close();
        }

        private void SpreadsheetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SpreadsheetList.SelectedItem != null)
            {
                SpreadsheetName.Text = SpreadsheetList.SelectedItem.ToString();
            }
        }

        private void SpreadsheetName_TextChanged(object sender, EventArgs e)
        {
            Join.Enabled = !string.IsNullOrWhiteSpace(SpreadsheetName.Text);
        }

        private void Connection_TextChanged(object sender, EventArgs eventArgs)
        {
            ButtonConnect.Enabled = !string.IsNullOrWhiteSpace(Username.Text) &&
                                    !string.IsNullOrWhiteSpace(IpAddress.Text) &&
                                    (Username.Text != _currentName || IpAddress.Text != _currentIP);
        }
    }
}