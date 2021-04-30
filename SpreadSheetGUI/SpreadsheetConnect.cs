// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021

using System;
using System.Windows.Forms;
using Control;

namespace SpreadSheetGUI
{
    public partial class SpreadsheetConnect : Form
    {
        private readonly Controller _clientController;
        private string _currentIP;
        private string _currentName;

        private SpreadsheetForm _form;

        /// <summary>
        ///     Creates a new Homepage
        /// </summary>
        public SpreadsheetConnect()
        {
            InitializeComponent();


            _clientController = new Controller();
            _clientController.Connected += Connected;
            _clientController.Error += Error;
            _clientController.Disconnected += Disconnected;
            _clientController.GetSpreadsheets += SetSpreadsheetNames;

            Username.TextChanged += Connection_TextChanged;
            IpAddress.TextChanged += Connection_TextChanged;
            SpreadsheetName.TextChanged += (sender, args) => SpreadsheetName_TextChanged();
        }

        /// <summary>
        ///     Connection listener: When client connects to server
        /// </summary>
        private void Connected()
        {
            Invoke(new MethodInvoker(
                () =>
                {
                    _currentName = Username.Text;
                    _currentIP = IpAddress.Text;
                    ButtonConnect.Enabled = false;
                    SpreadsheetName_TextChanged();
                }));
        }

        /// <summary>
        ///     Creates a warning on Error
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="title">Error Name</param>
        private void Error(string message, string title)
        {
            Invoke(new MethodInvoker(
                () => { SpreadsheetForm.Warning(message, title, SpreadsheetForm.WarningType.Error); }));
        }

        /// <summary>
        ///     Called When Client Disconnects
        /// </summary>
        private void Disconnected()
        {
            Invoke(new MethodInvoker(
                () =>
                {
                    _currentName = null;
                    _currentIP = null;
                    ButtonConnect.Enabled = true;
                    Join.Enabled = false;
                }));
        }

        /// <summary>
        ///     Called when spreadsheet names are received from server.
        ///     Displays spreadsheet names on a textbox.
        /// </summary>
        /// <param name="spreadsheetNames"></param>
        private void SetSpreadsheetNames(string[] spreadsheetNames)
        {
            Invoke(new MethodInvoker(
                () =>
                {
                    SpreadsheetList.Items.Clear();
                    if (spreadsheetNames.Length > 0)
                    {
                        foreach (string sheet in spreadsheetNames) SpreadsheetList.Items.Add(sheet);

                        SpreadsheetList.Focus();
                        SpreadsheetList.SelectedIndex = 0;
                    }
                    else
                    {
                        SpreadsheetList.Enabled = false;
                        SpreadsheetList.Items.Add("No spreadsheets have been created. Please create a new one.");
                        SpreadsheetName.Focus();
                    }
                }));
        }

        /// <summary>
        ///     ButtonConnect click listener: Connects client to Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            string username = Username.Text;
            if (string.IsNullOrWhiteSpace(username))
                SpreadsheetForm.Warning("Error: Please enter a username", "Empty Username Error",
                    SpreadsheetForm.WarningType.Error);

            string address = IpAddress.Text;
            if (string.IsNullOrWhiteSpace(address))
                SpreadsheetForm.Warning("Error: Please enter an IP Address", "Empty IP Address Error",
                    SpreadsheetForm.WarningType.Error);

            _clientController.Connect(username, address);
        }

        /// <summary>
        ///     Joins the spreadsheet that is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Join_Click(object sender, EventArgs e)
        {
            Hide();
            _form = new SpreadsheetForm(_clientController, SpreadsheetName.Text);
            _form.ShowDialog();
            Close();
        }

        /// <summary>
        ///     Listener for spreadsheet names list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpreadsheetList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SpreadsheetList.SelectedItem != null) SpreadsheetName.Text = SpreadsheetList.SelectedItem.ToString();
        }

        /// <summary>
        ///     Spreadsheet name selection listener
        /// </summary>
        private void SpreadsheetName_TextChanged()
        {
            if (_clientController.IsConnected)
                Join.Enabled = !string.IsNullOrWhiteSpace(SpreadsheetName.Text);
            AcceptButton = Join;
        }

        /// <summary>
        ///     Connection status listener
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void Connection_TextChanged(object sender, EventArgs eventArgs)
        {
            ButtonConnect.Enabled = !string.IsNullOrWhiteSpace(Username.Text) &&
                                    !string.IsNullOrWhiteSpace(IpAddress.Text) &&
                                    (Username.Text != _currentName || IpAddress.Text != _currentIP);
            AcceptButton = ButtonConnect;
        }
    }
}