using ImageMagick;
using NTwain;
using Oog.Core.Extensions;
using Oog.Tools.Factory;
using Oog.WarehouseScan.Command;
using Oog.WarehouseScan.Enumerations;

namespace Oog.WarehouseScan
{
    public partial class MainForm : Form
    {
        protected log4net.ILog Log { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string WarehouseReference { get; set; }

        private DocTypeEnum DocType { get; set; }

        private readonly List<ComboBox> AllComboBoxes = [];

        private readonly List<Image> ScannedImages = [];

        private bool isDownloading = false;

        private List<string> ExistingPdfPaths = [];

        public MainForm()
        {
            InitializeComponent();

            TextBoxInitializeInput();
            ComboBoxScannerInitialize();
            InitializeTypeCombo();

            LabelTextBoxTypeDetected.Text = string.Empty;
            LabelTextBoxError.Text = string.Empty;
            LabelOrderDetailsLine1.Text = string.Empty;
            LabelOrderDetailsLine2.Text = string.Empty;
            LabelOrderDetailsLine3.Text = string.Empty;
            labelHistory.Text = string.Empty;

            loadingGif.Visible = false;
            ButtonScan.Enabled = false;
            ButtonScan.BackColor = Color.FromArgb(224, 224, 224);

            ButtonSave.Enabled = false;
            ButtonSave.BackColor = Color.FromArgb(224, 224, 224);

            ButtonOpenExistingDocument.Visible = false;
            ButtonOpenExistingDocument.Enabled = false;

            PanelScans.AutoScroll = true;
            PanelScans.WrapContents = true;
            PanelScans.FlowDirection = FlowDirection.LeftToRight;

            progressBarDownload.Visible = false;
            labelDownload.Visible = false;

            TextBoxInput.TextChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(TextBoxInput.Text))
                {
                    if (AllComboBoxes.Count == 0)
                    {
                        InitializeTypeCombo();
                    }
                }
            };
        }

        private void ClearOrderDetails()
        {
            LabelTextBoxTypeDetected.Text = string.Empty;
            LabelTextBoxError.Text = string.Empty;
            LabelOrderDetailsLine1.Text = string.Empty;
            LabelOrderDetailsLine2.Text = string.Empty;
            LabelOrderDetailsLine3.Text = string.Empty;
            labelHistory.Text = string.Empty;
            ButtonOpenExistingDocument.Visible = false;
            ButtonOpenExistingDocument.Enabled = false;
            ExistingPdfPaths.Clear();
        }

        private void ComboBoxScannerInitialize()
        {
            Task.Run(() =>
            {
                try
                {
                    var command = OogFactory.GetService<FetchScannersCommand>();
                    command.Execute();

                    FetchScannersCommand.TwainSession.DataTransferred += TwainSession_DataTransferred;

                    ScannerListCombo.Invoke(() =>
                    {
                        ScannerListCombo.DataSource = command.Scanners;
                        ScannerListCombo.DisplayMember = "Name";

                        var selectedScanner = ScannerListCombo.SelectedItem;
                        if (selectedScanner != null)
                        {
                        }

                    });
                }
                catch (Exception ex)
                {
                    Log.Error("Erreur lors de l'initialisation des scanners", ex);
                }
            });
        }

        private void TextBoxInitializeInput()
        {
            TextBoxInput.KeyDown += TextBoxInput_KeyDown!;
            TextBoxInput.KeyPress += TextBoxAutoriseInput_KeyPress;
        }

        private void TextBoxAutoriseInput_KeyPress(object? sender, KeyPressEventArgs e)
        {
            bool valid = char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '-' || e.KeyChar == '\b';

            if (!valid)
            {
                e.Handled = true;
                Log.Debug($"Caractère non autorisé bloqué: {e.KeyChar}");
            }
            else if (e.KeyChar != '\b')
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void TextBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            TextBoxInputEvent(e);
        }

        private void ButtonValitdateTextBox_Click(object sender, EventArgs e)
        {
            Log.Debug("Clic sur le bouton de validation");
            TextBoxInputEvent(new KeyEventArgs(Keys.Enter));
        }

        private void TextBoxInputEvent(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                string input = TextBoxInput.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    ClearOrderDetails();

                    comboBoxSelectModeType.DataSource = null;
                    comboBoxSelectModeType.Items.Clear();
                    comboBoxSelectModeType.Text = "";

                    ButtonScan.Enabled = false;
                    ButtonScan.BackColor = Color.FromArgb(224, 224, 224);

                    ButtonOpenExistingDocument.Visible = false;
                    ButtonOpenExistingDocument.Enabled = false;
                    labelHistory.Text = string.Empty;

                    LabelTextBoxError.ForeColor = Color.Red;
                    LabelTextBoxError.Text = "Veuillez saisir une information avant de confirmer.";
                    Log.Debug($"Tentative d'une recherche avec un champ vide: {input}");

                    return;
                }

                loadingGif.Visible = true;
                ButtonValitdateTextBox.Enabled = false;
                LabelTextBoxTypeDetected.Text = string.Empty;
                ClearOrderDetails();

                Task.Run(() =>
                {
                    try
                    {
                        Log.Debug($"Début de la recherche pour l'entrée: {input}");

                        var detectCommand = OogFactory.GetService<SelectTypeFromInputCommand>();
                        detectCommand.InputText = input;
                        detectCommand.Execute();
                        DocType = detectCommand.DetectedDocumentType;

                        var searchCommand = OogFactory.GetService<SearchIdFromOrderCommand>();
                        searchCommand.DocType = DocType;
                        searchCommand.Id = input;
                        searchCommand.Execute();
                        WarehouseReference = searchCommand.WarehouseReference;
                        ExistingPdfPaths = searchCommand.ExistingPdfPaths;

                        bool hasData =
                            !string.IsNullOrWhiteSpace(searchCommand.Line1) ||
                            !string.IsNullOrWhiteSpace(searchCommand.Line2) ||
                            !string.IsNullOrWhiteSpace(searchCommand.Line3);

                        this.Invoke((Delegate)(() =>
                        {
                            LabelOrderDetailsLine1.Text = searchCommand.Line1;
                            LabelOrderDetailsLine2.Text = searchCommand.Line2;
                            LabelOrderDetailsLine3.Text = searchCommand.Line3;

                            if (DocType == DocTypeEnum.Unknown || !hasData)
                            {
                                LabelTextBoxError.ForeColor = Color.Red;
                                LabelTextBoxError.Text = "Le champ renseigné est inconnue.";
                                ButtonScan.Enabled = false;
                                ButtonScan.BackColor = Color.FromArgb(224, 224, 224);

                                comboBoxSelectModeType.DataSource = new List<DocumentType>();
                            }
                            else
                            {
                                UpdateScanPanel(DocType, searchCommand);
                                ButtonScan.Enabled = true;
                                ButtonScan.BackColor = Color.FromArgb(215, 228, 242);
                                LabelTextBoxTypeDetected.Text = $"{DocType.GetNameTranslated()}";

                                if (ExistingPdfPaths.Count > 0)
                                {
                                    ButtonOpenExistingDocument.Visible = true;
                                    ButtonOpenExistingDocument.Enabled = true;
                                    labelHistory.Text = "Cette référence a déjà été scannée :";
                                }
                            }

                            RefreshAllTypeComboBoxes();
                            loadingGif.Visible = false;
                            ButtonValitdateTextBox.Enabled = true;
                        }));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Erreur durant la détection du type", ex);
                        this.Invoke((Delegate)(() =>
                        {
                            LabelTextBoxError.ForeColor = Color.Red;
                            LabelTextBoxError.Text = "Erreur durant la détection du type";
                            loadingGif.Visible = false;
                            ButtonValitdateTextBox.Enabled = true;
                            ButtonScan.Enabled = false;
                            ButtonScan.BackColor = Color.FromArgb(224, 224, 224);

                            comboBoxSelectModeType.DataSource = new List<DocumentType>();
                        }));
                    }
                });
            }
        }

        private void ButtonScan_Click(object sender, EventArgs e)
        {
            var scanner = ScannerListCombo.SelectedItem as DataSource;

            if (scanner == null)
            {
                Log.Error("Aucun scanner sélectionné");
            }
            else
            {
                Task.Run(() =>
                {
                    try
                    {
                        Log.Debug("Lancement du scan des documents");

                        var command = OogFactory.GetService<ScanDocumentCommand>();
                        command.SelectedScanner = scanner;
                        command.TwainSession = FetchScannersCommand.TwainSession;
                        command.Execute();

                        Thread.Sleep(5000);
                        ButtonScan.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Erreur lors du lancement du scan avec {scanner.Name}", ex);
                    }
                });
            }
        }

        private void TwainSession_DataTransferred(object sender, NTwain.DataTransferredEventArgs e)
        {
            try
            {
                using (var image = new MagickImage(e.FileDataPath, new MagickReadSettings { Compression = CompressionMethod.BZip }))
                {
                    image.Format = MagickFormat.Jpeg;
                    var bitmap = image.ToBitmap();
                    DocTypeEnum docType;

                    string input = TextBoxInput.Text.Trim();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        docType = DocTypeEnum.Unknown;
                    }
                    else
                    {
                        var detectCommand = OogFactory.GetService<SelectTypeFromInputCommand>();
                        detectCommand.InputText = input;
                        detectCommand.Execute();
                        docType = detectCommand.DetectedDocumentType;
                    }

                    ScannedImages.Add(bitmap);
                    AddScannedPictureToPanel(bitmap, docType);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Erreur lors du traitement de l'image scannée: {e.FileDataPath}", ex);
            }
        }

        private void AddScannedPictureToPanel(Image img, DocTypeEnum docType)
        {
            PanelScans.Invoke(() =>
            {
                try
                {
                    Log.Debug($"Création d'une image scanné dans le panel : {docType}");

                    const int panelWidth = 280;
                    const int panelHeight = 350;
                    const int buttonSize = 25;
                    const int spacing = 5;
                    const int spacingX = 5;
                    const int spacingY = 5;
                    const int topBarHeight = 35;

                    int comboWidth = panelWidth - (buttonSize * 2) - (spacing * 4);

                    Panel container = CreateContainer(img, panelWidth, panelHeight, spacing);
                    PictureBox picture = CreatePicture(img, panelWidth, panelHeight, spacing, topBarHeight);
                    ComboBox comboPictureType = CreateComboPictureType(buttonSize, spacing, comboWidth);

                    var fetchTypeCommand = new FetchTypeScanDocCommand { DocType = docType };
                    fetchTypeCommand.Execute();
                    comboPictureType.DataSource = fetchTypeCommand.ScanTypes.Select(st => new DocumentType(st, st.GetNameTranslated())).ToList();
                    comboPictureType.DisplayMember = "Name";

                    Button buttonDelete = CreateDeleteButton(panelWidth, buttonSize, spacing);
                    buttonDelete.Click += (s, e) =>
                    {
                        PanelScans.Controls.Remove(container);
                        AllComboBoxes.Remove(comboPictureType);
                        ScannedImages.Remove(img);
                        Log.Debug($"Une image supprimée.");
                        UpdateGlobalTypeFromIndividualChoices();

                        if (PanelScans.Controls.Count == 0)
                        {
                            ButtonSave.Enabled = false;
                            ButtonSave.BackColor = Color.FromArgb(224, 224, 224);
                        }
                        Log.Debug($"Images restantes dans le panel du bas: {ScannedImages.Count}");
                    };

                    Button buttonRotate = CreateRotateButton(buttonSize, spacingX, spacingY);
                    buttonRotate.Click += (s, e) =>
                    {
                        var tag = (ScanTag)container.Tag;
                        tag.OrientationInDegree = (tag.OrientationInDegree == 0) ? 180 : 0;
                        picture.Image = RotatePicture(tag.Original, tag.OrientationInDegree);
                        Log.Debug($"Rotation de l'image à {tag.OrientationInDegree} degrés");
                    };

                    container.Controls.Add(picture);
                    container.Controls.Add(buttonRotate);
                    container.Controls.Add(comboPictureType);
                    container.Controls.Add(buttonDelete);

                    AllComboBoxes.Add(comboPictureType);

                    PanelScans.Controls.Add(container);

                    ButtonSave.Enabled = true;
                    ButtonSave.BackColor = Color.FromArgb(192, 255, 192);

                    UpdateGlobalTypeFromIndividualChoices();
                }
                catch (Exception ex)
                {
                    Log.Error("Erreur lors de l'ajout de l'image au panel", ex);
                }
            });
        }

        private void UpdateScanPanel(DocTypeEnum docType, SearchIdFromOrderCommand searchCommand)
        {
            PanelScans.Invoke((Delegate)(() =>
            {
                LabelTextBoxTypeDetected.ForeColor = docType != DocTypeEnum.Unknown ? Color.Green : Color.Red;
                LabelTextBoxTypeDetected.Text = docType.GetNameTranslated();
                ButtonScan.Enabled = docType != DocTypeEnum.Unknown;
                ButtonScan.BackColor = docType != DocTypeEnum.Unknown ? Color.FromArgb(215, 228, 242) : Color.FromArgb(215, 228, 242);

                LabelOrderDetailsLine1.Text = searchCommand.Line1;
                LabelOrderDetailsLine2.Text = searchCommand.Line2;
                LabelOrderDetailsLine3.Text = searchCommand.Line3;
                Log.Debug($"Résultat de la recherche : {searchCommand.Line1}, {searchCommand.Line2}, {searchCommand.Line3}");
            }));
        }

        private static PictureBox CreatePicture(Image img, int panelWidth, int panelHeight, int spacing, int topBarHeight)
        {
            return new PictureBox
            {
                Image = img,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = panelWidth - spacing * 2,
                Height = panelHeight - topBarHeight,
                Location = new Point(spacing, topBarHeight),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private static Button CreateRotateButton(int buttonSize, int spacingX, int spacingY)
        {
            var buttonRotate = new Button
            {
                Width = buttonSize,
                Height = buttonSize,
                Location = new Point(spacingX, spacingY),
                BackColor = Color.SteelBlue,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Text = "🔄",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            buttonRotate.FlatAppearance.BorderSize = 0;
            return buttonRotate;
        }

        private Image RotatePicture(Image picture, int angle)
        {
            var rotatedImage = new Bitmap(picture);
            if (angle == 180)
            {
                rotatedImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }

            return rotatedImage;
        }

        private static ComboBox CreateComboPictureType(int buttonSize, int spacing, int comboWidth)
        {
            return new ComboBox
            {
                Width = comboWidth,
                Location = new Point(buttonSize + spacing * 2, spacing),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
        }

        private static Button CreateDeleteButton(int panelWidth, int buttonSize, int spacing)
        {
            var result = new Button
            {
                Width = buttonSize,
                Height = buttonSize,
                Location = new Point(panelWidth - buttonSize - spacing, spacing),
                BackColor = Color.Crimson,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Text = "✕",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            result.FlatAppearance.BorderSize = 0;
            return result;
        }

        private static Panel CreateContainer(Image img, int panelWidth, int panelHeight, int spacing)
        {      
            return new Panel
            {
                Width = panelWidth,
                Height = panelHeight,
                Margin = new Padding(5),
                BackColor = Color.SeaShell,
                Tag = new ScanTag(img, 0)
            };
        }

        private class ScanTag(Image Original, int OrientationInDegree)
        {
            public Image Original { get; set; } = Original;

            public int OrientationInDegree { get; set; } = OrientationInDegree;
        }

        private class DocumentType(ScanTypeEnum type, string name)
        {
            public ScanTypeEnum Type { get; } = type;

            public string Name { get; } = name;
        }

        private class ImageAndScanType(Image image, ScanTypeEnum scanType)
        {
            public Image image { get; } = image;

            public ScanTypeEnum scanType { get; } = scanType;
        }

        private void InitializeTypeCombo()
        {
            comboBoxSelectModeType.DisplayMember = "Name";
            comboBoxSelectModeType.SelectedIndexChanged += ComboBoxSelectModeType_Selected;

            comboBoxSelectModeType.DataSource = new List<DocumentType>();
        }

        private void ComboBoxSelectModeType_Selected(object? sender, EventArgs e)
        {
            var chosenItem = comboBoxSelectModeType.SelectedItem as DocumentType;

            if (chosenItem != null)
            {
                foreach (var comboBox in AllComboBoxes)
                {
                    var match = comboBox.Items
                        .Cast<DocumentType>()
                        .FirstOrDefault(x => x.Type == chosenItem.Type);

                    if (match != null)
                    {
                        comboBox.SelectedItem = match;
                    }
                    else if (comboBox.Items.Count > 0)
                    {
                        comboBox.SelectedIndex = 0;
                    }
                }
            }
        }

        private void UpdateGlobalTypeFromIndividualChoices()
        {
            if (AllComboBoxes.Count == 0 || DocType == DocTypeEnum.Unknown)
            {
                comboBoxSelectModeType.DataSource = new List<DocumentType>();
            }
            else
            {
                var allTypes = new HashSet<ScanTypeEnum>();
                foreach (var combo in AllComboBoxes)
                {
                    var items = combo.Items.Cast<DocumentType>();
                    foreach (var item in items)
                    {
                        allTypes.Add(item.Type);
                    }
                }

                var typeList = allTypes.Select(st => new DocumentType(st, st.GetNameTranslated())).ToList();
                comboBoxSelectModeType.DataSource = typeList;

                if (typeList.Count > 0)
                {
                    comboBoxSelectModeType.SelectedIndex = 0;
                }
            }
        }

        private void RefreshAllTypeComboBoxes()
        {
            var fetchTypeCommand = new FetchTypeScanDocCommand { DocType = DocType };
            fetchTypeCommand.Execute();

            var availableTypes = fetchTypeCommand.ScanTypes;

            foreach (var comboBox in AllComboBoxes)
            {
                var currentSelectedItem = comboBox.SelectedItem as DocumentType;

                var filteredTypes = availableTypes
                    .Select(st => new DocumentType(st, st.GetNameTranslated()))
                    .ToList();

                comboBox.DataSource = filteredTypes;
                comboBox.DisplayMember = "Name";

                if (currentSelectedItem != null &&
                    filteredTypes.Any(t => t.Type == currentSelectedItem.Type))
                {
                    var match = filteredTypes.FirstOrDefault(t => t.Type == currentSelectedItem.Type);
                    comboBox.SelectedItem = match;
                }
                else if (filteredTypes.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }
            }

            if (DocType != DocTypeEnum.Unknown && availableTypes.Any())
            {
                var typeList = availableTypes.Select(st => new DocumentType(st, st.GetNameTranslated())).ToList();
                comboBoxSelectModeType.DataSource = typeList;

                if (typeList.Count > 0)
                {
                    comboBoxSelectModeType.SelectedIndex = 0;
                }
            }
            else
            {
                comboBoxSelectModeType.DataSource = new List<DocumentType>();
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {

            List<ImageAndScanType> imagesAndTypes = new List<ImageAndScanType>();

            foreach (var panel in PanelScans.Controls.OfType<Panel>())
            {
                var comboBox = panel.Controls.OfType<ComboBox>().First();
                var pictureBox = panel.Controls.OfType<PictureBox>().First();

                var selectedDocumentType = comboBox.SelectedItem as DocumentType;

                if (selectedDocumentType == null)
                {
                    continue;
                }

                var scanType = selectedDocumentType.Type;
                var image = pictureBox.Image;

                var imageCopy = new Bitmap(image);

                imagesAndTypes.Add(new ImageAndScanType(imageCopy, scanType));
            }

            if (imagesAndTypes.Count == 0)
            {
                return;
            }

            StartDownloadAnimation();

            Task.Run(() =>
            {
                try
                {
                    var imagesByTypes = imagesAndTypes.GroupBy(it => it.scanType);
                    int totalGroups = imagesByTypes.Count();
                    int completedGroups = 0;

                    foreach (var imagesForOneType in imagesByTypes)
                    {
                        Log.Debug($"Début du téléchargement : {imagesForOneType.Key}, {imagesForOneType.Count()} image(s)");

                        var imageToPdfCommand = OogFactory.GetService<ImageToPdfCommand>();
                        imageToPdfCommand.ImagesToConvert = imagesForOneType.Select(it => it.image).ToList();
                        imageToPdfCommand.Execute();

                        var saveCommand = OogFactory.GetService<SaveCommand>();
                        saveCommand.WarehouseReference = WarehouseReference;
                        saveCommand.InternalReference = TextBoxInput.Text;
                        saveCommand.DocType = DocType;
                        saveCommand.ScanType = imagesForOneType.Key;
                        saveCommand.PdfStream = imageToPdfCommand.MemoryStream;

                        saveCommand.FileName = $"{TextBoxInput.Text}_{imagesForOneType.Key}_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.pdf";

                        saveCommand.Images = imagesForOneType.Select(it => it.image).ToList();

                        saveCommand.Execute();

                        completedGroups++;

                        int progressPercentage = (int)((completedGroups / (double)totalGroups) * 100);
                        this.Invoke((MethodInvoker)(() =>
                        {
                            progressBarDownload.Value = Math.Min(progressPercentage, 100);
                        }));
                    }

                    this.Invoke(() =>
                    {
                        CompleteDownloadAnimation();
                        PanelScans.Controls.Clear();
                        AllComboBoxes.Clear();
                        ScannedImages.Clear();
                        ButtonSave.Enabled = false;
                        ButtonSave.BackColor = Color.FromArgb(224, 224, 224);

                        comboBoxSelectModeType.DataSource = new List<DocumentType>();

                        foreach (var item in imagesAndTypes)
                        {
                            item.image.Dispose();
                        }

                    });
                    Log.Debug("Sauvegarde terminée avec succès.");
                }
                catch (Exception ex)
                {
                    Log.Error("Erreur lors de la sauvegarde des scans", ex);
                    this.Invoke(() =>
                    {
                        CompleteDownloadAnimation();
                        MessageBox.Show($"Erreur lors de la sauvegarde: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        private void StartDownloadAnimation()
        {
            isDownloading = true;
            progressBarDownload.Value = 0;
            progressBarDownload.Visible = true;
            labelDownload.Visible = true;

            labelDownload.Text = "Téléchargement en cours...";
        }

        private void CompleteDownloadAnimation()
        {
            isDownloading = false;
            progressBarDownload.Value = 100;

            Task.Delay(2000).ContinueWith(t =>
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    progressBarDownload.Visible = false;
                    labelDownload.Visible = false;
                    progressBarDownload.Value = 0;
                    labelDownload.Text = string.Empty;
                }));
            });

            labelDownload.Text = "Téléchargement terminé !";
        }

        private void ButtonOpenExistingDocument_Click(object sender, EventArgs e)
        {
            if (ExistingPdfPaths.Count == 0)
            {
                MessageBox.Show("Aucun document existant à ouvrir.", "Information");
            }
            else
            {
                try
                {
                    var existingFiles = ExistingPdfPaths.Where(File.Exists).ToList();

                    if (existingFiles.Count == 0)
                    {
                        MessageBox.Show("Les fichiers PDF référencés n'existent plus.");
                    }
                    else if (existingFiles.Count == 1)
                    {
                        // Ouvre le fichier PDF avec l'application par défaut du système
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = existingFiles[0],
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        ShowPdfSelectionDialog(existingFiles);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Erreur lors de l'ouverture du PDF", ex);
                    MessageBox.Show($"Erreur lors de l'ouverture du PDF : {ex.Message}", "Erreur");
                }
            }
        }

        private void ShowPdfSelectionDialog(List<string> pdfFiles)
        {
            using (var selectionForm = new PdfSelectionForm(pdfFiles))
            {
                if (selectionForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(selectionForm.SelectedFilePath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = selectionForm.SelectedFilePath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Impossible d'ouvrir le fichier sélectionné", ex);
                        MessageBox.Show($"Impossible d'ouvrir le fichier : {ex.Message}",
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
