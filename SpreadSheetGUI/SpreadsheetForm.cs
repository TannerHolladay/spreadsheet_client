// Written by Tanner Holladay for CS 3500 on October 8th, 2020

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Control;
using SpreadsheetUtilities;
using SS;
using SSJson;

namespace SpreadSheetGUI
{
    public partial class SpreadsheetForm : Form
    {
        // Name of the file to save recent saves to.
        private const string RecentSaveFile = "RecentSaves.xml";


        private static HelpBox _helpBox;
        private static List<string> _recentSaves;


        private Spreadsheet _spreadsheet;
        private string _selection;
        private int _col;
        private int _row;
        private string _currentFile;

        private Controller clientController;

        private string CurrentFile
        {
            get => _currentFile;
            set => SetCurrentFile(value);
        }

        /// <summary>
        /// Valid if the value is any letter A-Z followed by a number 1-99
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <returns>True if the value is valid</returns>
        private static bool IsValid(string name) => Regex.IsMatch(name, @"^[A-Z][1-9][0-9]?$");

        /// <summary>
        /// Converts inputs to uppercase so both lower and upper can be valid variables.
        /// </summary>
        /// <param name="name">Value to be normalized</param>
        /// <returns>The uppercase value</returns>
        private static string Normalize(string name) => name.ToUpper();

        /// <summary>
        /// Spreadsheet form that allows you to enter input. You can enter formulas by
        /// using "=" before the formula ex. (=2+A1)
        /// </summary>
        public SpreadsheetForm(Controller controller)
        {
            InitializeComponent();

            _helpBox = new HelpBox();
            clientController = controller;

            AcceptButton = ButtonUpdate;
            LabelError.Visible = false;
        }

        public void OnConnected(string spreadsheetName)
        {
            Text = spreadsheetName;
            _spreadsheet = new Spreadsheet(IsValid, Normalize, "1.0");

            KeyPreview = true;

            spreadsheetPanel.SelectionChanged += CellSelectionChange;
            CellSelectionChange(spreadsheetPanel);
        }


        public void OnClientDisconnect(Disconnected d)
        {
            Invoke(new MethodInvoker(
                () =>
                {
                    LabelError.Text = "User " + d.getUserID() + " disconnected";
                    LabelError.Visible = true;
                }));
        }

        public void OnIDReceived(int id)
        {
            spreadsheetPanel.setID(id);
            Console.WriteLine(id);
        }

        /// <summary>
        /// Updates current online selections or adds a new one if not found
        /// </summary>
        /// <param name="c"></param>
        public void OnNewCellSelection(CellSelected selected)
        {
            var col = Regex.Match(selected.getCellName(), @"^[A-Z]").Value[0] - 'A';
            var row = int.Parse(Regex.Match(selected.getCellName(), @"\d*$").Value);

            spreadsheetPanel.UpdateOnlineSelection(col, row - 1, selected.getClientID(), selected.getClientName());
        }

        public void OnlineCellEdited(CellUpdated c)
        {
            Invoke(new MethodInvoker(
                () =>
                {
                    try
                    {
                        var updated = _spreadsheet.SetContentsOfCell(c.getCellName(), c.getContents());
                        foreach (var cell in updated)
                        {
                            UpdateCell(cell);
                        }
                    }
                    catch (Exception exception)
                    {
                        LabelError.Text = exception.Message;
                        LabelError.Visible = true;
                    }
                }
            ));
        }
        
        public void OnServerShutdown(ServerShutdownError error)
        {
            Warning(error.getMessage(), "Server Shutdown", WarningType.Error);
            Close();
        }

        public void OnRequestError(RequestError error)
        {
            Warning(error.getMessage() + "\nCell: " + error.getCellName(), "Invalid Request", WarningType.Error);
        }

        /// <summary>
        /// Gets the current spreadsheet and provides a dialog to save it
        /// </summary>
        private void SaveSpreadSheetAs()
        {
            if (!_spreadsheet.Changed) return;
            if (CurrentFile != null)
            {
                SaveSpreadsheet(CurrentFile);
            }
            else
            {
                SaveFileDialog.ShowDialog();
            }
        }

        // Enum Types for the different warning choices
        public enum WarningType
        {
            Error,
            Question,
            Warning
        }

        /// <summary>
        /// Helper method to make it easier to send a popup Dialog
        /// </summary>
        /// <param name="message">Main content of the popup</param>
        /// <param name="title">The title at the top of the dialog</param>
        /// <param name="type">What type of dialog to show</param>
        /// <returns>True if the user selects Yes or Ok</returns>
        public static bool Warning(string message, string title, WarningType type)
        {
            MessageBoxButtons buttons;
            MessageBoxIcon icon;
            switch (type)
            {
                case WarningType.Error:
                    icon = MessageBoxIcon.Warning;
                    buttons = MessageBoxButtons.OK;
                    break;
                case WarningType.Question:
                    icon = MessageBoxIcon.Question;
                    buttons = MessageBoxButtons.YesNo;
                    break;
                case WarningType.Warning:
                    icon = MessageBoxIcon.Warning;
                    buttons = MessageBoxButtons.YesNo;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, @"Message type invalid");
            }

            var messageBox = MessageBox.Show(
                message,
                title,
                buttons,
                icon,
                MessageBoxDefaultButton.Button2
            );
            return messageBox == DialogResult.OK || messageBox == DialogResult.Yes;
        }

        private void UpdateCell(string cell)
        {
            var col = Regex.Match(cell, @"^[A-Z]").Value[0] - 'A';
            var row = int.Parse(Regex.Match(cell, @"\d*$").Value);
            spreadsheetPanel.SetValue(col, row - 1, _spreadsheet.GetCellValue(cell).ToString());
        }


        /// <summary>
        /// Saves a spreadsheet selected from a file selection dialog
        /// </summary>
        /// <param name="filename">Name of the file to be saved to</param>
        private bool SaveSpreadsheet(string filename)
        {
            try
            {
                CurrentFile = filename;
                _spreadsheet.Save(CurrentFile);
                return false;
            }
            catch (Exception error)
            {
                Warning(error.Message, "File Save Error!", WarningType.Error);
                return true;
            }
        }


        /// <summary>
        /// Updates the selected cell on the spreadsheet
        /// </summary>
        private void EditSelectedCell()
        {
            EditCell edit = new EditCell();
            edit.setCellName(_selection);
            edit.setContents(BoxContents.Text);

            SelectCell selected = new SelectCell();
            selected.setCellName(_selection);

            clientController.SendUpdatesToServer(selected);
            clientController.SendUpdatesToServer(edit);
        }

        /// <summary>
        /// Loads information from the selected cell when changed.
        /// </summary>
        /// <param name="ssp"></param>
        private void CellSelectionChange(SpreadsheetPanel ssp)
        {
            BoxContents.Focus();
            LabelError.Visible = false;
            ssp.GetSelection(out _col, out _row);
            if (ssp.GetValue(_col, _row, out var value))
                BoxValue.Text = value;
            else
                BoxValue.Clear();

            _selection = $"{(char) ('A' + _col)}{_row + 1}";
            ButtonUpdate.Text = $@"Update: {_selection}";
            BoxSelected.Text = _selection;

            try
            {
                var contents = _spreadsheet.GetCellContents(_selection);
                if (contents is Formula) contents = $"={contents}";
                BoxContents.Text = contents.ToString();
            }
            catch (Exception e)
            {
                LabelError.Visible = true;
                LabelError.Text = e.Message;
                return;
            }

            // This should send the update to the server (do we need to do this in the try block?)
            SelectCell selected = new SelectCell();
            selected.setCellName(_selection);

            // Tell the server that this client selected a new cell.
            // Client ID not needed prior to selecting a cell (not mentioned in protocol document) < - Double check this

            clientController.SendUpdatesToServer(selected);
        }


        /// <summary>
        /// Sets the current file of the program. If set, then the program will save
        /// to this program.
        /// </summary>
        /// <param name="value"></param>
        private void SetCurrentFile(string value)
        {
            _currentFile = value;

            ButtonSave.Enabled = true;
        }

        #region Controller Events

        private void ButtonUpdate_Click(object sender, EventArgs e) =>
            EditSelectedCell(); // Event thrown selecting the update button to update the selected cell

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e) =>
            OpenFileDialog.ShowDialog(); // Event thrown from clicking on the load menu item in File

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e) =>
            SaveFileDialog.ShowDialog(); // Event thrown from clicking on the save menu item in File

        private void SaveFileDialog_FileOk(object sender, CancelEventArgs e) =>
            e.Cancel = SaveSpreadsheet(SaveFileDialog.FileName); // Event thrown when choosing a file to save to

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e) =>
            _helpBox.ShowDialog(); // Event thrown when selecting the help button on the tool strip

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) =>
            Close(); // Event thrown when choosing the close button in File

        private void ButtonSave_Click(object sender, EventArgs e) =>
            SaveSpreadSheetAs(); // When the save button is clicked, saves to the last saved file

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // Save spreadsheet using CTRL+S
        {
            if (keyData != (Keys.Control | Keys.S)) return false;
            SaveSpreadSheetAs();
            return true;
        }

        #endregion

        private void UndoButton_Click(object sender, EventArgs e)
        {
            clientController.SendUpdatesToServer(new UndoCell());
        }

        private void RevertButton_Click(object sender, EventArgs e)
        {
            RevertCell revert = new RevertCell();
            revert.setCellName(_selection);
            clientController.SendUpdatesToServer(revert);
        }
    }
}