namespace OpenBus.WinForm
{
    partial class MainForm
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
            this.startGameButton = new System.Windows.Forms.Button();
            this.mapList = new System.Windows.Forms.ComboBox();
            this.mapLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // startGameButton
            // 
            this.startGameButton.Location = new System.Drawing.Point(1050, 614);
            this.startGameButton.Name = "startGameButton";
            this.startGameButton.Size = new System.Drawing.Size(202, 55);
            this.startGameButton.TabIndex = 0;
            this.startGameButton.Text = "startGameButton";
            this.startGameButton.UseVisualStyleBackColor = true;
            this.startGameButton.Click += new System.EventHandler(this.startGameButton_Click);
            // 
            // mapList
            // 
            this.mapList.FormattingEnabled = true;
            this.mapList.Location = new System.Drawing.Point(128, 12);
            this.mapList.Name = "mapList";
            this.mapList.Size = new System.Drawing.Size(293, 21);
            this.mapList.TabIndex = 1;
            // 
            // mapLabel
            // 
            this.mapLabel.AutoSize = true;
            this.mapLabel.Location = new System.Drawing.Point(12, 15);
            this.mapLabel.Name = "mapLabel";
            this.mapLabel.Size = new System.Drawing.Size(35, 13);
            this.mapLabel.TabIndex = 2;
            this.mapLabel.Text = "label1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.mapLabel);
            this.Controls.Add(this.mapList);
            this.Controls.Add(this.startGameButton);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startGameButton;
        private System.Windows.Forms.ComboBox mapList;
        private System.Windows.Forms.Label mapLabel;
    }
}