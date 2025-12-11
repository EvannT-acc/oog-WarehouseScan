using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace Oog.WarehouseScan
{
    public partial class PdfSelectionForm : Form
    {
        private List<string> pdfFiles;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedFilePath { get; private set; }

        public PdfSelectionForm(List<string> pdfFiles)
        {
            this.pdfFiles = pdfFiles;
            InitializeComponent();
            InitializeFileList();
        }

        private void InitializeFileList()
        {
            listBoxFiles.Items.Clear();
            foreach (var file in pdfFiles)
            {
                listBoxFiles.Items.Add(Path.GetFileName(file));
            }

            if (listBoxFiles.Items.Count > 0)
            {
                listBoxFiles.SelectedIndex = 0;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedIndex >= 0)
            {
                SelectedFilePath = pdfFiles[listBoxFiles.SelectedIndex];
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}