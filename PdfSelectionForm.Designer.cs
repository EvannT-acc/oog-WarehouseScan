namespace Oog.WarehouseScan
{
    partial class PdfSelectionForm
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
            panelMain = new Panel();
            panelButtons = new Panel();
            btnCancel = new Button();
            btnOpen = new Button();
            listBoxFiles = new ListBox();
            labelTitle = new Label();
            panelMain.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.Controls.Add(panelButtons);
            panelMain.Controls.Add(listBoxFiles);
            panelMain.Controls.Add(labelTitle);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 0);
            panelMain.Name = "panelMain";
            panelMain.Padding = new Padding(10);
            panelMain.Size = new Size(484, 286);
            panelMain.TabIndex = 0;
            // 
            // panelButtons
            // 
            panelButtons.BackColor = Color.Silver;
            panelButtons.BorderStyle = BorderStyle.FixedSingle;
            panelButtons.Controls.Add(btnCancel);
            panelButtons.Controls.Add(btnOpen);
            panelButtons.Location = new Point(10, 224);
            panelButtons.Name = "panelButtons";
            panelButtons.Padding = new Padding(10);
            panelButtons.Size = new Size(464, 52);
            panelButtons.TabIndex = 2;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(224, 224, 224);
            btnCancel.FlatAppearance.BorderColor = Color.DimGray;
            btnCancel.Location = new Point(378, 10);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 30);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Annuler";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOpen
            // 
            btnOpen.BackColor = SystemColors.GradientInactiveCaption;
            btnOpen.FlatAppearance.BorderColor = Color.DimGray;
            btnOpen.Location = new Point(281, 10);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(93, 30);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "Ouvrir";
            btnOpen.UseVisualStyleBackColor = false;
            btnOpen.Click += btnOpen_Click;
            // 
            // listBoxFiles
            // 
            listBoxFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxFiles.BackColor = Color.WhiteSmoke;
            listBoxFiles.BorderStyle = BorderStyle.FixedSingle;
            listBoxFiles.Cursor = Cursors.Hand;
            listBoxFiles.Font = new Font("Palatino Linotype", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            listBoxFiles.ForeColor = Color.Black;
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.Location = new Point(10, 45);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.ScrollAlwaysVisible = true;
            listBoxFiles.Size = new Size(464, 170);
            listBoxFiles.TabIndex = 1;
            // 
            // labelTitle
            // 
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Palatino Linotype", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTitle.ForeColor = Color.FromArgb(60, 60, 60);
            labelTitle.Location = new Point(8, 14);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(287, 27);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Sélectionnez un PDF à ouvrir :";
            // 
            // PdfSelectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGray;
            ClientSize = new Size(484, 286);
            Controls.Add(panelMain);
            ForeColor = Color.FromArgb(30, 30, 30);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PdfSelectionForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Sélection de PDF";
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMain;
        private Label labelTitle;
        private ListBox listBoxFiles;
        private Panel panelButtons;
        private Button btnOpen;
        private Button btnCancel;
    }
}