using Oog.Core.Extensions;
using Oog.WarehouseScan.Enumerations;
using System.ComponentModel;
using System.Security.Principal;

namespace Oog.WarehouseScan
{
    public partial class PdfSelectionForm : Form
    {
        private List<string> pdfFiles;
        private string currentUserName;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedFilePath { get; private set; }

        public PdfSelectionForm(List<string> pdfFiles)
        {
            this.pdfFiles = pdfFiles;

            currentUserName = WindowsIdentity.GetCurrent().Name.Split('\\').LastOrDefault() ?? WindowsIdentity.GetCurrent().Name;

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
                var fileName = System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name);

                var parts = fileName.Split('_');

                if (parts.Length >= 6)
                {
                    var scanType = parts[1];
                    var dateStr = parts[2];
                    var timeStr = parts[3];
                    var userName = parts[4];

                    string translatedType = scanType;
                    if (Enum.TryParse(scanType, out ScanTypeEnum scanTypeEnum))
                    {
                        translatedType = scanTypeEnum.GetNameTranslated();
                    }

                    var formattedDate = dateStr.Replace("-", "/");
                    var timeParts = timeStr.Split('-');
                    var hourMinute = timeParts.Length >= 2 ? $"{timeParts[0]}:{timeParts[1]}" : timeStr;

                    var displayText = $"{translatedType} - {formattedDate} {hourMinute} - {userName}";

                    if (DateTime.TryParseExact($"{dateStr} {timeStr}", "dd-MM-yyyy HH-mm-ss",
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
                else if (parts.Length >= 5)
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

                    var displayText = $"{translatedType} - {formattedDate} {hourMinute} - {currentUserName}";

                    if (DateTime.TryParseExact($"{dateStr} {timeStr}", "dd-MM-yyyy HH-mm-ss",
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
                else if (parts.Length == 4)
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

                    var displayText = $"{translatedType} - {formattedDate} {hourMinute} - {currentUserName}";

                    if (DateTime.TryParseExact($"{dateStr} {timeStr}", "dd-MM-yyyy HH-mm-ss",
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
                else
                {
                    var displayText = $"{fileInfo.Name} - {currentUserName}";
                    filesWithDates.Add((filePath, DateTime.MaxValue, displayText));
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