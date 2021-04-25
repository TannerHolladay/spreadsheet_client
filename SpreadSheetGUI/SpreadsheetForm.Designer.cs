﻿using System.Windows.Forms;

namespace SpreadSheetGUI
{
    partial class SpreadsheetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ButtonUpdate = new System.Windows.Forms.Button();
            this.BoxContents = new System.Windows.Forms.TextBox();
            this.BoxValue = new System.Windows.Forms.TextBox();
            this.LabelSelected = new System.Windows.Forms.Label();
            this.LabelError = new System.Windows.Forms.Label();
            this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonSave = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.RecentItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ItemAutoLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.spreadsheetPanel = new SS.SpreadsheetPanel();
            this.LabelValue = new System.Windows.Forms.Label();
            this.BoxSelected = new System.Windows.Forms.TextBox();
            this.Connect_Button = new System.Windows.Forms.Button();
            this.Username_TextBox = new System.Windows.Forms.TextBox();
            this.IPAddress_TextBox = new System.Windows.Forms.TextBox();
            this.Usernam_Label = new System.Windows.Forms.Label();
            this.IPAddress_Label = new System.Windows.Forms.Label();
            this.ContextMenuStrip1.SuspendLayout();
            this.MenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonUpdate
            // 
            this.ButtonUpdate.Location = new System.Drawing.Point(211, 25);
            this.ButtonUpdate.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonUpdate.Name = "ButtonUpdate";
            this.ButtonUpdate.Size = new System.Drawing.Size(87, 28);
            this.ButtonUpdate.TabIndex = 1;
            this.ButtonUpdate.Text = "Update: A1";
            this.ButtonUpdate.UseVisualStyleBackColor = true;
            this.ButtonUpdate.Click += new System.EventHandler(this.ButtonUpdate_Click);
            // 
            // BoxContents
            // 
            this.BoxContents.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.BoxContents.Location = new System.Drawing.Point(9, 25);
            this.BoxContents.Margin = new System.Windows.Forms.Padding(2);
            this.BoxContents.Name = "BoxContents";
            this.BoxContents.Size = new System.Drawing.Size(198, 29);
            this.BoxContents.TabIndex = 2;
            // 
            // BoxValue
            // 
            this.BoxValue.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BoxValue.Cursor = System.Windows.Forms.Cursors.Default;
            this.BoxValue.Location = new System.Drawing.Point(144, 497);
            this.BoxValue.Margin = new System.Windows.Forms.Padding(2);
            this.BoxValue.Name = "BoxValue";
            this.BoxValue.ReadOnly = true;
            this.BoxValue.Size = new System.Drawing.Size(187, 20);
            this.BoxValue.TabIndex = 3;
            this.BoxValue.Text = "BoxValue";
            // 
            // LabelSelected
            // 
            this.LabelSelected.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LabelSelected.AutoSize = true;
            this.LabelSelected.BackColor = System.Drawing.Color.Transparent;
            this.LabelSelected.Location = new System.Drawing.Point(9, 499);
            this.LabelSelected.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelSelected.Name = "LabelSelected";
            this.LabelSelected.Size = new System.Drawing.Size(52, 13);
            this.LabelSelected.TabIndex = 4;
            this.LabelSelected.Text = "Selected:";
            this.LabelSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LabelError
            // 
            this.LabelError.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelError.Enabled = false;
            this.LabelError.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.LabelError.ForeColor = System.Drawing.Color.Firebrick;
            this.LabelError.Location = new System.Drawing.Point(343, 497);
            this.LabelError.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelError.Name = "LabelError";
            this.LabelError.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelError.Size = new System.Drawing.Size(439, 18);
            this.LabelError.TabIndex = 5;
            this.LabelError.Text = "Error Message";
            this.LabelError.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.SaveToolStripMenuItem, this.LoadToolStripMenuItem});
            this.ContextMenuStrip1.Name = "contextMenuStrip1";
            this.ContextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.SaveToolStripMenuItem.Text = "Save";
            // 
            // LoadToolStripMenuItem
            // 
            this.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem";
            this.LoadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.LoadToolStripMenuItem.Text = "Load";
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.fileToolStripMenuItem, this.optionsToolStripMenuItem, this.helpToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.MenuStrip1.Size = new System.Drawing.Size(791, 24);
            this.MenuStrip1.TabIndex = 6;
            this.MenuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.newToolStripMenuItem, this.ButtonSave, this.ButtonSaveAs, this.loadToolStripMenuItem1, this.RecentItem, this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            // 
            // ButtonSave
            // 
            this.ButtonSave.Enabled = false;
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(136, 22);
            this.ButtonSave.Text = "Save";
            this.ButtonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // ButtonSaveAs
            // 
            this.ButtonSaveAs.Name = "ButtonSaveAs";
            this.ButtonSaveAs.Size = new System.Drawing.Size(136, 22);
            this.ButtonSaveAs.Text = "Save As...";
            this.ButtonSaveAs.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem1
            // 
            this.loadToolStripMenuItem1.Name = "loadToolStripMenuItem1";
            this.loadToolStripMenuItem1.Size = new System.Drawing.Size(136, 22);
            this.loadToolStripMenuItem1.Text = "Open";
            this.loadToolStripMenuItem1.Click += new System.EventHandler(this.LoadToolStripMenuItem_Click);
            // 
            // RecentItem
            // 
            this.RecentItem.Name = "RecentItem";
            this.RecentItem.Size = new System.Drawing.Size(136, 22);
            this.RecentItem.Text = "Recent Files";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {this.ItemAutoLoad});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // ItemAutoLoad
            // 
            this.ItemAutoLoad.Checked = true;
            this.ItemAutoLoad.CheckOnClick = true;
            this.ItemAutoLoad.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ItemAutoLoad.Name = "ItemAutoLoad";
            this.ItemAutoLoad.Size = new System.Drawing.Size(163, 22);
            this.ItemAutoLoad.Text = "Startup Last Save";
            this.ItemAutoLoad.Click += new System.EventHandler(this.startupRecentSaveToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.HelpToolStripMenuItem_Click);
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.DefaultExt = "sprd";
            this.OpenFileDialog.FileName = "spreadsheet";
            this.OpenFileDialog.Filter = "Spreadsheet Files|*.sprd|All files|*.*";
            this.OpenFileDialog.Title = "Load File To Spreadsheet";
            this.OpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDialog_FileOk);
            // 
            // SaveFileDialog
            // 
            this.SaveFileDialog.DefaultExt = "sprd";
            this.SaveFileDialog.Filter = "Spreadsheet Files|*.sprd|All files|*.*";
            this.SaveFileDialog.Title = "Save Spreadsheet To File";
            this.SaveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFileDialog_FileOk);
            // 
            // spreadsheetPanel
            // 
            this.spreadsheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel.Location = new System.Drawing.Point(9, 58);
            this.spreadsheetPanel.Margin = new System.Windows.Forms.Padding(2);
            this.spreadsheetPanel.Name = "spreadsheetPanel";
            this.spreadsheetPanel.Size = new System.Drawing.Size(773, 429);
            this.spreadsheetPanel.TabIndex = 0;
            // 
            // LabelValue
            // 
            this.LabelValue.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LabelValue.AutoSize = true;
            this.LabelValue.BackColor = System.Drawing.Color.Transparent;
            this.LabelValue.Location = new System.Drawing.Point(105, 499);
            this.LabelValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelValue.Name = "LabelValue";
            this.LabelValue.Size = new System.Drawing.Size(37, 13);
            this.LabelValue.TabIndex = 7;
            this.LabelValue.Text = "Value:";
            this.LabelValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BoxSelected
            // 
            this.BoxSelected.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BoxSelected.Cursor = System.Windows.Forms.Cursors.Default;
            this.BoxSelected.Location = new System.Drawing.Point(61, 497);
            this.BoxSelected.Margin = new System.Windows.Forms.Padding(2);
            this.BoxSelected.Name = "BoxSelected";
            this.BoxSelected.ReadOnly = true;
            this.BoxSelected.Size = new System.Drawing.Size(29, 20);
            this.BoxSelected.TabIndex = 8;
            this.BoxSelected.Text = "Z99";
            this.BoxSelected.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Connect_Button
            // 
            this.Connect_Button.Location = new System.Drawing.Point(325, 26);
            this.Connect_Button.Margin = new System.Windows.Forms.Padding(1);
            this.Connect_Button.Name = "Connect_Button";
            this.Connect_Button.Size = new System.Drawing.Size(54, 24);
            this.Connect_Button.TabIndex = 9;
            this.Connect_Button.Text = "Connect";
            this.Connect_Button.UseVisualStyleBackColor = true;
            // 
            // Username_TextBox
            // 
            this.Username_TextBox.Location = new System.Drawing.Point(448, 34);
            this.Username_TextBox.Margin = new System.Windows.Forms.Padding(1);
            this.Username_TextBox.Name = "Username_TextBox";
            this.Username_TextBox.Size = new System.Drawing.Size(66, 20);
            this.Username_TextBox.TabIndex = 10;
            this.Username_TextBox.Text = "Dude";
            // 
            // IPAddress_TextBox
            // 
            this.IPAddress_TextBox.Location = new System.Drawing.Point(581, 35);
            this.IPAddress_TextBox.Margin = new System.Windows.Forms.Padding(1);
            this.IPAddress_TextBox.Name = "IPAddress_TextBox";
            this.IPAddress_TextBox.Size = new System.Drawing.Size(72, 20);
            this.IPAddress_TextBox.TabIndex = 11;
            this.IPAddress_TextBox.Text = "localhost";
            // 
            // Usernam_Label
            // 
            this.Usernam_Label.AutoSize = true;
            this.Usernam_Label.Location = new System.Drawing.Point(390, 35);
            this.Usernam_Label.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.Usernam_Label.Name = "Usernam_Label";
            this.Usernam_Label.Size = new System.Drawing.Size(58, 13);
            this.Usernam_Label.TabIndex = 12;
            this.Usernam_Label.Text = "Username:";
            // 
            // IPAddress_Label
            // 
            this.IPAddress_Label.AutoSize = true;
            this.IPAddress_Label.Location = new System.Drawing.Point(521, 37);
            this.IPAddress_Label.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.IPAddress_Label.Name = "IPAddress_Label";
            this.IPAddress_Label.Size = new System.Drawing.Size(60, 13);
            this.IPAddress_Label.TabIndex = 13;
            this.IPAddress_Label.Text = "Ip Address:";
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 521);
            this.Controls.Add(this.IPAddress_Label);
            this.Controls.Add(this.Usernam_Label);
            this.Controls.Add(this.IPAddress_TextBox);
            this.Controls.Add(this.Username_TextBox);
            this.Controls.Add(this.Connect_Button);
            this.Controls.Add(this.BoxSelected);
            this.Controls.Add(this.LabelValue);
            this.Controls.Add(this.MenuStrip1);
            this.Controls.Add(this.LabelError);
            this.Controls.Add(this.LabelSelected);
            this.Controls.Add(this.BoxValue);
            this.Controls.Add(this.BoxContents);
            this.Controls.Add(this.ButtonUpdate);
            this.Controls.Add(this.spreadsheetPanel);
            this.KeyPreview = true;
            this.MainMenuStrip = this.MenuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(471, 225);
            this.Name = "SpreadsheetForm";
            this.ShowIcon = false;
            this.Text = "Spreadsheet";
            this.ContextMenuStrip1.ResumeLayout(false);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel;
        private System.Windows.Forms.Button ButtonUpdate;
        private System.Windows.Forms.TextBox BoxContents;
        private System.Windows.Forms.TextBox BoxValue;
        private System.Windows.Forms.Label LabelSelected;
        private System.Windows.Forms.Label LabelError;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadToolStripMenuItem;
        private System.Windows.Forms.MenuStrip MenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ButtonSaveAs;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem1;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Label LabelValue;
        private System.Windows.Forms.TextBox BoxSelected;
        private System.Windows.Forms.ToolStripMenuItem RecentItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ItemAutoLoad;
        private ToolStripMenuItem ButtonSave;
        private Button Connect_Button;
        private TextBox Username_TextBox;
        private TextBox IPAddress_TextBox;
        private Label Usernam_Label;
        private Label IPAddress_Label;
    }
}

