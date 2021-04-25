using System.ComponentModel;

namespace SpreadSheetGUI
{
    partial class SpreadsheetConnect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.ButtonConnect = new System.Windows.Forms.Button();
            this.LabelUsername = new System.Windows.Forms.Label();
            this.Username = new System.Windows.Forms.TextBox();
            this.IpAddress = new System.Windows.Forms.TextBox();
            this.LabelIpAddress = new System.Windows.Forms.Label();
            this.SpreadsheetList = new System.Windows.Forms.ListBox();
            this.LabelCreatJoin = new System.Windows.Forms.Label();
            this.LabelSpreadsheet = new System.Windows.Forms.Label();
            this.SpreadsheetName = new System.Windows.Forms.TextBox();
            this.Join = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ButtonConnect
            // 
            this.ButtonConnect.Enabled = false;
            this.ButtonConnect.Location = new System.Drawing.Point(674, 12);
            this.ButtonConnect.Name = "ButtonConnect";
            this.ButtonConnect.Size = new System.Drawing.Size(114, 29);
            this.ButtonConnect.TabIndex = 0;
            this.ButtonConnect.Text = "Connect";
            this.ButtonConnect.UseVisualStyleBackColor = true;
            this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // LabelUsername
            // 
            this.LabelUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.LabelUsername.Location = new System.Drawing.Point(12, 18);
            this.LabelUsername.Name = "LabelUsername";
            this.LabelUsername.Size = new System.Drawing.Size(87, 19);
            this.LabelUsername.TabIndex = 1;
            this.LabelUsername.Text = "Username:";
            // 
            // Username
            // 
            this.Username.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.Username.Location = new System.Drawing.Point(104, 12);
            this.Username.Margin = new System.Windows.Forms.Padding(2);
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(198, 29);
            this.Username.TabIndex = 2;
            // 
            // IpAddress
            // 
            this.IpAddress.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.IpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.IpAddress.Location = new System.Drawing.Point(403, 12);
            this.IpAddress.Margin = new System.Windows.Forms.Padding(2);
            this.IpAddress.Name = "IpAddress";
            this.IpAddress.Size = new System.Drawing.Size(266, 29);
            this.IpAddress.TabIndex = 4;
            // 
            // LabelIpAddress
            // 
            this.LabelIpAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.LabelIpAddress.Location = new System.Drawing.Point(307, 18);
            this.LabelIpAddress.Name = "LabelIpAddress";
            this.LabelIpAddress.Size = new System.Drawing.Size(91, 19);
            this.LabelIpAddress.TabIndex = 3;
            this.LabelIpAddress.Text = "IP Address:";
            // 
            // SpreadsheetList
            // 
            this.SpreadsheetList.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.SpreadsheetList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.SpreadsheetList.FormattingEnabled = true;
            this.SpreadsheetList.ItemHeight = 20;
            this.SpreadsheetList.Location = new System.Drawing.Point(12, 80);
            this.SpreadsheetList.Name = "SpreadsheetList";
            this.SpreadsheetList.Size = new System.Drawing.Size(776, 164);
            this.SpreadsheetList.TabIndex = 5;
            this.SpreadsheetList.SelectedIndexChanged += new System.EventHandler(this.SpreadsheetList_SelectedIndexChanged);
            // 
            // LabelCreatJoin
            // 
            this.LabelCreatJoin.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.LabelCreatJoin.Location = new System.Drawing.Point(12, 54);
            this.LabelCreatJoin.Name = "LabelCreatJoin";
            this.LabelCreatJoin.Size = new System.Drawing.Size(290, 19);
            this.LabelCreatJoin.TabIndex = 6;
            this.LabelCreatJoin.Text = "Create New or Join A Spreadsheet";
            // 
            // LabelSpreadsheet
            // 
            this.LabelSpreadsheet.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LabelSpreadsheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.LabelSpreadsheet.Location = new System.Drawing.Point(12, 268);
            this.LabelSpreadsheet.Name = "LabelSpreadsheet";
            this.LabelSpreadsheet.Size = new System.Drawing.Size(106, 19);
            this.LabelSpreadsheet.TabIndex = 8;
            this.LabelSpreadsheet.Text = "Spreadsheet:";
            // 
            // SpreadsheetName
            // 
            this.SpreadsheetName.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.SpreadsheetName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.SpreadsheetName.Location = new System.Drawing.Point(115, 262);
            this.SpreadsheetName.Margin = new System.Windows.Forms.Padding(2);
            this.SpreadsheetName.Name = "SpreadsheetName";
            this.SpreadsheetName.Size = new System.Drawing.Size(497, 29);
            this.SpreadsheetName.TabIndex = 9;
            this.SpreadsheetName.TextChanged += new System.EventHandler(this.SpreadsheetName_TextChanged);
            // 
            // Join
            // 
            this.Join.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Join.Enabled = false;
            this.Join.Location = new System.Drawing.Point(617, 262);
            this.Join.Name = "Join";
            this.Join.Size = new System.Drawing.Size(171, 29);
            this.Join.TabIndex = 10;
            this.Join.Text = "Create/Join";
            this.Join.UseVisualStyleBackColor = true;
            this.Join.Click += new System.EventHandler(this.Join_Click);
            // 
            // SpreadsheetConnect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 311);
            this.Controls.Add(this.Join);
            this.Controls.Add(this.SpreadsheetName);
            this.Controls.Add(this.LabelSpreadsheet);
            this.Controls.Add(this.LabelCreatJoin);
            this.Controls.Add(this.SpreadsheetList);
            this.Controls.Add(this.IpAddress);
            this.Controls.Add(this.LabelIpAddress);
            this.Controls.Add(this.Username);
            this.Controls.Add(this.LabelUsername);
            this.Controls.Add(this.ButtonConnect);
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "SpreadsheetConnect";
            this.Text = "Spreadsheet Server";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button Join;

        private System.Windows.Forms.TextBox SpreadsheetName;

        private System.Windows.Forms.Label LabelSpreadsheet;

        private System.Windows.Forms.Label LabelCreatJoin;

        private System.Windows.Forms.ListBox SpreadsheetList;

        private System.Windows.Forms.Label LabelIpAddress;
        private System.Windows.Forms.TextBox IpAddress;

        private System.Windows.Forms.TextBox Username;

        private System.Windows.Forms.Label LabelUsername;

        private System.Windows.Forms.Button ButtonConnect;

        #endregion
    }
}