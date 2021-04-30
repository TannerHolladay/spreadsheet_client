// Written by Tanner Holladay, Noah Carlson, Abbey Nelson, Sergio Remigio, Travis Schnider, Jimmy Glasscock for CS 3505 on April 28, 2021

using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Control;
using SpreadsheetUtilities;
using SS;
using SSJson;

namespace SpreadSheetGUI
{
    public partial class SpreadsheetForm : Form
    {
        // Enum Types for the different warning choices
        public enum WarningType
        {
            Error,
            Question,
            Warning
        }

        private static HelpBox _helpBox;

        private readonly Controller _clientController;

        private readonly Spreadsheet _spreadsheet;
        private int _col;
        private int _row;
        private string _selection;

        /// <summary>
        ///     Spreadsheet form that allows you to enter input. You can enter formulas by
        ///     using "=" before the formula ex. (=2+A1)
        /// </summary>
        public SpreadsheetForm(Controller controller, string spreadsheetName)
        {
            InitializeComponent();
            _spreadsheet = new Spreadsheet(IsValid, Normalize, "1.0");
            Text = spreadsheetName;

            _helpBox = new HelpBox();
            _clientController = controller;

            AcceptButton = ButtonUpdate;
            LabelError.Visible = false;
            HandleCreated += OnShown;
        }

        /// <summary>
        ///     Valid if the value is any letter A-Z followed by a number 1-99
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <returns>True if the value is valid</returns>
        private static bool IsValid(string name)
        {
            return Regex.IsMatch(name, @"^[A-Z][1-9][0-9]?$");
        }

        /// <summary>
        ///     Converts inputs to uppercase so both lower and upper can be valid variables.
        /// </summary>
        /// <param name="name">Value to be normalized</param>
        /// <returns>The uppercase value</returns>
        private static string Normalize(string name)
        {
            return name.ToUpper();
        }

        // Method invoker requires the form to be loaded, so it subscribes all events and then sends the spreadsheet name to the server
        private void OnShown(object sender, EventArgs eventArgs)
        {
            KeyPreview = true;

            _clientController.IDReceive += OnIDReceived;
            _clientController.EditCell += OnlineCellEdited;
            _clientController.ServerShutdown += OnServerShutdown;
            _clientController.RequestError += OnRequestError;
            _clientController.ClientDisconnected += OnClientDisconnect;
            _clientController.SelectCell += OnNewCellSelection;
            spreadsheetPanel.SelectionChanged += SendSelectedUpdate;

            _clientController.SendSpreadsheetRequest(Text);
        }

        /// <summary>
        ///     Client Disconnect Listener
        /// </summary>
        /// <param name="d">disconnected Json object</param>
        private void OnClientDisconnect(Disconnected d)
        {
            Invoke(new MethodInvoker(
                () =>
                {
                    LabelError.Text = "User " + d.GetUserID() + " disconnected";
                    LabelError.Visible = true;
                }));
        }

        /// <summary>
        ///     ID received listener
        /// </summary>
        /// <param name="id">id of the client</param>
        private void OnIDReceived(int id)
        {
            spreadsheetPanel.SetID(id);
        }

        /// <summary>
        ///     Updates current online selections or adds a new one if not found
        /// </summary>
        /// <param name="selected">CellSelected Json object</param>
        private void OnNewCellSelection(CellSelected selected)
        {
            int col = Regex.Match(selected.GetCellName(), @"^[A-Z]").Value[0] - 'A';
            int row = int.Parse(Regex.Match(selected.GetCellName(), @"\d*$").Value);

            spreadsheetPanel.UpdateOnlineSelection(col, row - 1, selected.GetClientID(), selected.GetClientName());
        }

        /// <summary>
        ///     Online clients cell edit listener
        /// </summary>
        /// <param name="c">CellUpdated Json object</param>
        private void OnlineCellEdited(CellUpdated c)
        {
            Invoke(new MethodInvoker(
                () =>
                {
                    try
                    {
                        var updated = _spreadsheet.SetContentsOfCell(c.GetCellName(), c.GetContents());
                        foreach (string cell in updated) UpdateCell(cell);

                        CellSelectionChange(spreadsheetPanel);
                    }
                    catch (Exception exception)
                    {
                        LabelError.Text = exception.Message;
                        LabelError.Visible = true;
                    }
                }
            ));
        }

        /// <summary>
        ///     Server Shutdown listener: Displays a warning
        /// </summary>
        /// <param name="error"></param>
        private void OnServerShutdown(ServerShutdownError error)
        {
            Warning(error.GetMessage(), "Server Shutdown", WarningType.Error);
            Close();
        }

        /// <summary>
        ///     Request Error listener: Displays and warning
        /// </summary>
        /// <param name="error"></param>
        private static void OnRequestError(RequestError error)
        {
            Warning(error.GetMessage() + "\nCell: " + error.GetCellName(), "Invalid Request", WarningType.Error);
        }

        /// <summary>
        ///     Helper method to make it easier to send a popup Dialog
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

        /// <summary>
        ///     Updates the cellValue of Cell
        /// </summary>
        /// <param name="cell">Name of cell</param>
        private void UpdateCell(string cell)
        {
            int col = Regex.Match(cell, @"^[A-Z]").Value[0] - 'A';
            int row = int.Parse(Regex.Match(cell, @"\d*$").Value);
            spreadsheetPanel.SetValue(col, row - 1, _spreadsheet.GetCellValue(cell).ToString());
        }


        /// <summary>
        ///     Updates the selected cell on the spreadsheet
        /// </summary>
        private void EditSelectedCell()
        {
            var edit = new EditCell();
            edit.SetCellName(_selection);
            edit.SetContents(BoxContents.Text);

            _clientController.SendUpdatesToServer(edit);
        }

        /// <summary>
        ///     Loads information from the selected cell when changed.
        /// </summary>
        /// <param name="ssp"></param>
        private void CellSelectionChange(SpreadsheetPanel ssp)
        {
            LabelError.Visible = false;
            ssp.GetSelection(out _col, out _row);
            if (ssp.GetValue(_col, _row, out string value))
                BoxValue.Text = value;
            else
                BoxValue.Clear();

            _selection = $"{(char) ('A' + _col)}{_row + 1}";
            ButtonUpdate.Text = $@"Update: {_selection}";
            BoxSelected.Text = _selection;

            try
            {
                object contents = _spreadsheet.GetCellContents(_selection);
                if (contents is Formula) contents = $"={contents}";
                BoxContents.Text = contents.ToString();
            }
            catch (Exception e)
            {
                LabelError.Visible = true;
                LabelError.Text = e.Message;
            }
        }

        private void SendSelectedUpdate(SpreadsheetPanel ssp)
        {
            BoxContents.Focus();
            CellSelectionChange(ssp);

            // This should send the update to the server (do we need to do this in the try block?)
            var selected = new SelectCell();
            selected.SetCellName(_selection);

            // Tell the server that this client selected a new cell.
            // Client ID not needed prior to selecting a cell (not mentioned in protocol document) < - Double check this

            _clientController.SendUpdatesToServer(selected);
        }


        /// <summary>
        ///     Undo Button Click listener: Sends Undo message to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            _clientController.SendUpdatesToServer(new UndoCell());
        }

        /// <summary>
        ///     Revert Button Click listener: Sends revert message to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RevertButton_Click(object sender, EventArgs e)
        {
            var revert = new RevertCell();
            revert.SetCellName(_selection);
            _clientController.SendUpdatesToServer(revert);
        }

        #region Controller Events

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            EditSelectedCell(); // Event thrown selecting the update button to update the selected cell
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog.ShowDialog(); // Event thrown from clicking on the load menu item in File
        }

        private void HelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _helpBox.ShowDialog(); // Event thrown when selecting the help button on the tool strip
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // Save spreadsheet using CTRL+S
        {
            if (keyData != (Keys.Control | Keys.Z)) return false;
            UndoButton.Select();
            return true;
        }

        #endregion
    }
}