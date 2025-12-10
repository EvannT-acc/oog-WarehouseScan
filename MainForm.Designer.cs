namespace Oog.WarehouseScan
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            TextBoxInput = new TextBox();
            ButtonValitdateTextBox = new Button();
            loadingGif = new PictureBox();
            LabelInputHelp = new Label();
            comboBoxSelectModeType = new ComboBox();
            LabelTextBoxError = new Label();
            LabelTextBoxTypeDetected = new Label();
            LabelOrderDetailsLine1 = new Label();
            LabelOrderDetailsLine2 = new Label();
            LabelOrderDetailsLine3 = new Label();
            LabelScanners = new Label();
            ScannerListCombo = new ComboBox();
            ButtonScan = new Button();
            PanelScans = new FlowLayoutPanel();
            ButtonSave = new Button();
            progressBarDownload = new ProgressBar();
            labelDownload = new Label();
            LabelComboBoxSelectModeTypeInfo = new Label();
            ButtonOpenExistingDocument = new Button();
            labelHistory = new Label();
            ((System.ComponentModel.ISupportInitialize)loadingGif).BeginInit();
            SuspendLayout();
            // 
            // TextBoxInput
            // 
            TextBoxInput.BackColor = Color.WhiteSmoke;
            TextBoxInput.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(TextBoxInput, "TextBoxInput");
            TextBoxInput.Name = "TextBoxInput";
            // 
            // ButtonValitdateTextBox
            // 
            ButtonValitdateTextBox.BackColor = SystemColors.GradientInactiveCaption;
            ButtonValitdateTextBox.Cursor = Cursors.Hand;
            resources.ApplyResources(ButtonValitdateTextBox, "ButtonValitdateTextBox");
            ButtonValitdateTextBox.Name = "ButtonValitdateTextBox";
            ButtonValitdateTextBox.UseVisualStyleBackColor = false;
            ButtonValitdateTextBox.Click += ButtonValitdateTextBox_Click;
            // 
            // loadingGif
            // 
            loadingGif.BackColor = Color.Transparent;
            resources.ApplyResources(loadingGif, "loadingGif");
            loadingGif.Name = "loadingGif";
            loadingGif.TabStop = false;
            // 
            // LabelInputHelp
            // 
            resources.ApplyResources(LabelInputHelp, "LabelInputHelp");
            LabelInputHelp.ForeColor = Color.FromArgb(90, 90, 90);
            LabelInputHelp.Name = "LabelInputHelp";
            // 
            // comboBoxSelectModeType
            // 
            comboBoxSelectModeType.BackColor = Color.SeaShell;
            comboBoxSelectModeType.Cursor = Cursors.Hand;
            comboBoxSelectModeType.DropDownStyle = ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBoxSelectModeType, "comboBoxSelectModeType");
            comboBoxSelectModeType.ForeColor = Color.Black;
            comboBoxSelectModeType.Name = "comboBoxSelectModeType";
            // 
            // LabelTextBoxError
            // 
            resources.ApplyResources(LabelTextBoxError, "LabelTextBoxError");
            LabelTextBoxError.ForeColor = Color.Black;
            LabelTextBoxError.Name = "LabelTextBoxError";
            // 
            // LabelTextBoxTypeDetected
            // 
            resources.ApplyResources(LabelTextBoxTypeDetected, "LabelTextBoxTypeDetected");
            LabelTextBoxTypeDetected.ForeColor = Color.Black;
            LabelTextBoxTypeDetected.Name = "LabelTextBoxTypeDetected";
            // 
            // LabelOrderDetailsLine1
            // 
            resources.ApplyResources(LabelOrderDetailsLine1, "LabelOrderDetailsLine1");
            LabelOrderDetailsLine1.ForeColor = Color.FromArgb(60, 60, 60);
            LabelOrderDetailsLine1.Name = "LabelOrderDetailsLine1";
            // 
            // LabelOrderDetailsLine2
            // 
            resources.ApplyResources(LabelOrderDetailsLine2, "LabelOrderDetailsLine2");
            LabelOrderDetailsLine2.ForeColor = Color.FromArgb(60, 60, 60);
            LabelOrderDetailsLine2.Name = "LabelOrderDetailsLine2";
            // 
            // LabelOrderDetailsLine3
            // 
            resources.ApplyResources(LabelOrderDetailsLine3, "LabelOrderDetailsLine3");
            LabelOrderDetailsLine3.ForeColor = Color.FromArgb(60, 60, 60);
            LabelOrderDetailsLine3.Name = "LabelOrderDetailsLine3";
            // 
            // LabelScanners
            // 
            resources.ApplyResources(LabelScanners, "LabelScanners");
            LabelScanners.ForeColor = Color.FromArgb(45, 45, 45);
            LabelScanners.Name = "LabelScanners";
            // 
            // ScannerListCombo
            // 
            resources.ApplyResources(ScannerListCombo, "ScannerListCombo");
            ScannerListCombo.BackColor = Color.WhiteSmoke;
            ScannerListCombo.Cursor = Cursors.Hand;
            ScannerListCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            ScannerListCombo.ForeColor = Color.Black;
            ScannerListCombo.Name = "ScannerListCombo";
            // 
            // ButtonScan
            // 
            resources.ApplyResources(ButtonScan, "ButtonScan");
            ButtonScan.BackColor = Color.FromArgb(224, 224, 224);
            ButtonScan.Cursor = Cursors.Hand;
            ButtonScan.FlatAppearance.BorderColor = Color.DimGray;
            ButtonScan.ForeColor = Color.Black;
            ButtonScan.Name = "ButtonScan";
            ButtonScan.UseVisualStyleBackColor = false;
            ButtonScan.Click += ButtonScan_Click;
            // 
            // PanelScans
            // 
            resources.ApplyResources(PanelScans, "PanelScans");
            PanelScans.BackColor = Color.SeaShell;
            PanelScans.BorderStyle = BorderStyle.FixedSingle;
            PanelScans.Name = "PanelScans";
            // 
            // ButtonSave
            // 
            resources.ApplyResources(ButtonSave, "ButtonSave");
            ButtonSave.BackColor = Color.FromArgb(224, 224, 224);
            ButtonSave.Cursor = Cursors.Hand;
            ButtonSave.FlatAppearance.BorderColor = Color.DimGray;
            ButtonSave.ForeColor = Color.Black;
            ButtonSave.Name = "ButtonSave";
            ButtonSave.UseVisualStyleBackColor = false;
            ButtonSave.Click += ButtonSave_Click;
            // 
            // progressBarDownload
            // 
            progressBarDownload.BackColor = Color.LightGray;
            progressBarDownload.ForeColor = Color.FromArgb(128, 255, 128);
            resources.ApplyResources(progressBarDownload, "progressBarDownload");
            progressBarDownload.Name = "progressBarDownload";
            // 
            // labelDownload
            // 
            resources.ApplyResources(labelDownload, "labelDownload");
            labelDownload.BackColor = Color.WhiteSmoke;
            labelDownload.Name = "labelDownload";
            // 
            // LabelComboBoxSelectModeTypeInfo
            // 
            resources.ApplyResources(LabelComboBoxSelectModeTypeInfo, "LabelComboBoxSelectModeTypeInfo");
            LabelComboBoxSelectModeTypeInfo.ForeColor = Color.FromArgb(90, 90, 90);
            LabelComboBoxSelectModeTypeInfo.Name = "LabelComboBoxSelectModeTypeInfo";
            // 
            // ButtonOpenExistingDocument
            // 
            ButtonOpenExistingDocument.BackColor = Color.FromArgb(255, 255, 192);
            ButtonOpenExistingDocument.Cursor = Cursors.Hand;
            ButtonOpenExistingDocument.FlatAppearance.BorderColor = Color.DimGray;
            resources.ApplyResources(ButtonOpenExistingDocument, "ButtonOpenExistingDocument");
            ButtonOpenExistingDocument.Name = "ButtonOpenExistingDocument";
            ButtonOpenExistingDocument.UseVisualStyleBackColor = false;
            ButtonOpenExistingDocument.Click += ButtonOpenExistingDocument_Click;
            // 
            // labelHistory
            // 
            resources.ApplyResources(labelHistory, "labelHistory");
            labelHistory.ForeColor = Color.FromArgb(60, 60, 60);
            labelHistory.Name = "labelHistory";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGray;
            Controls.Add(labelHistory);
            Controls.Add(ButtonOpenExistingDocument);
            Controls.Add(LabelComboBoxSelectModeTypeInfo);
            Controls.Add(labelDownload);
            Controls.Add(progressBarDownload);
            Controls.Add(LabelTextBoxError);
            Controls.Add(comboBoxSelectModeType);
            Controls.Add(ButtonSave);
            Controls.Add(loadingGif);
            Controls.Add(ButtonValitdateTextBox);
            Controls.Add(PanelScans);
            Controls.Add(ButtonScan);
            Controls.Add(LabelInputHelp);
            Controls.Add(LabelOrderDetailsLine3);
            Controls.Add(LabelOrderDetailsLine2);
            Controls.Add(LabelOrderDetailsLine1);
            Controls.Add(LabelTextBoxTypeDetected);
            Controls.Add(TextBoxInput);
            Controls.Add(LabelScanners);
            Controls.Add(ScannerListCombo);
            ForeColor = Color.FromArgb(30, 30, 30);
            Name = "MainForm";
            ((System.ComponentModel.ISupportInitialize)loadingGif).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private Label LabelInputHelp;
        private Label LabelTextBoxError;
        private Label LabelTextBoxTypeDetected;
        private Label LabelOrderDetailsLine1;
        private Label LabelOrderDetailsLine2;
        private Label LabelOrderDetailsLine3;
        private Label LabelScanners;
        private Label labelDownload;
        private Label LabelComboBoxSelectModeTypeInfo;
        private Label labelHistory;
        private TextBox TextBoxInput;
        private Button ButtonValitdateTextBox;
        private Button ButtonOpenExistingDocument;
        private Button ButtonScan;
        private Button ButtonSave;
        private FlowLayoutPanel PanelScans;
        private PictureBox loadingGif;
        private ComboBox comboBoxSelectModeType;
        private ComboBox ScannerListCombo;
        private ProgressBar progressBarDownload;
    }
}