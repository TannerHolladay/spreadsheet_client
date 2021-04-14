
namespace SpreadSheetGUI
{
    partial class SpreadsheetList
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
            this.NamesListBox = new System.Windows.Forms.ListBox();
            this.SelectedNameTextBox = new System.Windows.Forms.TextBox();
            this.SendNameButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NamesListBox
            // 
            this.NamesListBox.FormattingEnabled = true;
            this.NamesListBox.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.NamesListBox.Location = new System.Drawing.Point(152, 55);
            this.NamesListBox.Name = "NamesListBox";
            this.NamesListBox.Size = new System.Drawing.Size(512, 290);
            this.NamesListBox.TabIndex = 0;
            this.NamesListBox.SelectedIndexChanged += new System.EventHandler(this.NamesListBox_SelectedIndexChanged);
            // 
            // SelectedNameTextBox
            // 
            this.SelectedNameTextBox.Location = new System.Drawing.Point(152, 363);
            this.SelectedNameTextBox.Name = "SelectedNameTextBox";
            this.SelectedNameTextBox.Size = new System.Drawing.Size(415, 20);
            this.SelectedNameTextBox.TabIndex = 1;
            // 
            // SendNameButton
            // 
            this.SendNameButton.Location = new System.Drawing.Point(589, 363);
            this.SendNameButton.Name = "SendNameButton";
            this.SendNameButton.Size = new System.Drawing.Size(75, 23);
            this.SendNameButton.TabIndex = 2;
            this.SendNameButton.Text = "Send";
            this.SendNameButton.UseVisualStyleBackColor = true;
            this.SendNameButton.Click += new System.EventHandler(this.SendNameButton_Click);
            // 
            // SpreadsheetList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SendNameButton);
            this.Controls.Add(this.SelectedNameTextBox);
            this.Controls.Add(this.NamesListBox);
            this.Name = "SpreadsheetList";
            this.Text = "SpreadsheetList";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox NamesListBox;
        private System.Windows.Forms.TextBox SelectedNameTextBox;
        private System.Windows.Forms.Button SendNameButton;
    }
}