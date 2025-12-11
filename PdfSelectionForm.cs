using Oog.Core.Extensions;
using Oog.WarehouseScan.Enumerations;
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

            var filesWithDates = new List<(string filePath, DateTime date, string displayText)>();

            foreach (var filePath in pdfFiles)
            {
                var fileInfo = new FileInfo(filePath);
                var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

                var parts = fileName.Split('_');

                if (parts.Length >= 4)
                {
                    var scanType = parts[1];
                    var dateStr = parts[2];
                    var timeStr = parts[3];

                    string translatedType = scanType;
                    if (Enum.TryParse(scanType, out ScanTypeEnum scanTypeEnum))
                    {
                        translatedType = scanTypeEnum.GetNameTranslated();
                    }

                    var formattedDate = dateStr.Replace("-", "/");

                    var timeParts = timeStr.Split('-');
                    var hourMinute = timeParts.Length >= 2 ? $"{timeParts[0]}:{timeParts[1]}" : timeStr;

                    var displayText = $"{translatedType} - {formattedDate} {hourMinute}";

                    if (DateTime.TryParseExact($"{dateStr} {timeStr}",
                        "dd-MM-yyyy HH-mm-ss",
                        null,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime fileDate))
                    {
                        filesWithDates.Add((filePath, fileDate, displayText));
                    }
                    else
                    {
                        filesWithDates.Add((filePath, DateTime.MaxValue, displayText));
                    }
                }
                else if (parts.Length == 3)
                {
                    var scanType = parts[1];
                    var dateTimeStr = parts[2];

                    string translatedType = scanType;
                    if (Enum.TryParse(scanType, out ScanTypeEnum scanTypeEnum))
                    {
                        translatedType = scanTypeEnum.GetNameTranslated();
                    }

                    string displayText;
                    DateTime fileDate = DateTime.MaxValue;

                    if (dateTimeStr.Contains("_"))
                    {
                        var dateTimeParts = dateTimeStr.Split('_');
                        if (dateTimeParts.Length >= 2)
                        {
                            var date = dateTimeParts[0].Replace("-", "/");
                            var time = dateTimeParts[1].Replace("-", ":");
                            var timeParts = time.Split(':');
                            var hourMinute = timeParts.Length >= 2 ? $"{timeParts[0]}:{timeParts[1]}" : time;
                            displayText = $"{translatedType} - {date} {hourMinute}";

                            if (DateTime.TryParseExact($"{dateTimeParts[0]} {dateTimeParts[1]}",
                                "dd-MM-yyyy HH-mm-ss",
                                null,
                                System.Globalization.DateTimeStyles.None,
                                out fileDate))
                            {
                                filesWithDates.Add((filePath, fileDate, displayText));
                            }
                        }
                        else
                        {
                            displayText = $"{translatedType} - {dateTimeStr.Replace("-", "/")}";
                            filesWithDates.Add((filePath, DateTime.MaxValue, displayText));
                        }
                    }
                    else
                    {
                        displayText = $"{translatedType} - {dateTimeStr.Replace("-", "/")}";
                        filesWithDates.Add((filePath, DateTime.MaxValue, displayText));
                    }
                }
                else
                {
                    filesWithDates.Add((filePath, DateTime.MaxValue, fileInfo.Name));
                }
            }

            var sortedFiles = filesWithDates.OrderBy(f => f.date).ToList();

            foreach (var file in sortedFiles)
            {
                listBoxFiles.Items.Add(file.displayText);
            }

            pdfFiles = sortedFiles.Select(f => f.filePath).ToList();

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