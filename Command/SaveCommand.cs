using Microsoft.Practices.EnterpriseLibrary.Validation;
using Oog.Core.Extensions;
using Oog.Domain.Catalog.WebServices.Results;
using Oog.WarehouseScan.Enumerations;
using Oog.WebService.Core.EntryPoints;
using Oog.WindowsForms.Services;

namespace Oog.WarehouseScan.Command
{
    internal class SaveCommand : Domain.Command.Command
    {
        protected log4net.ILog Log { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DocTypeEnum DocType { get; set; }
        public ScanTypeEnum ScanType { get; set; }
        public string WarehouseReference { get; set; }
        public string InternalReference { get; set; }
        public MemoryStream PdfStream { get; set; }
        public string FileName { get; set; }
        public List<Image?> Images { get; internal set; }
        public string Path { get; private set; }
        public string SageDocumentType { get; private set; }

        protected override void OnExtendValidation(ValidationResults vr)
        {
            if (PdfStream == null || PdfStream.Length == 0)
            {
                vr.AddResult(new ValidationResult("Le flux PDF est vide ou null", this, nameof(PdfStream), null, null));
            }

            if (string.IsNullOrWhiteSpace(FileName))
            {
                vr.AddResult(new ValidationResult("Le nom de fichier est obligatoire", this, nameof(FileName), null, null));
            }
        }

        protected override void ExecuteWhenValidated()
        {
            string path = DetermineStoragePath();
            Log.Debug($"Chemin de stockage déterminé : {path}");

            if (!Directory.Exists(path))
            {
                Log.Warn($"Le répertoire n'existe pas, création : {path}");
                try
                {
                    Directory.CreateDirectory(path);
                    Log.Info($"Répertoire créé avec succès : {path}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Erreur lors de la création du répertoire {path}", ex);
                    throw;
                }
            }
            string fullPath = System.IO.Path.Combine(path, FileName);

            try
            {
                PdfStream.Position = 0;

                using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    PdfStream.CopyTo(fileStream);
                    fileStream.Flush();
                }

                var result = EndPoints.ScanDocument<ScanDocumentResult>(SageDocumentType, InternalReference, FileName, EnumExtensions.GetDescription(ScanType));

                if (!result.IsSuccess || result.ReturnCode != 1)
                {
                    new WinformsDialogService().ShowError(new WindowsForms.Services.Configurators.WinformsDialogServiceConfigurator
                    {
                        Caption = Translation.Scanner.ErrorCaption,
                        Message = Translation.Scanner.UploadCustomerScanDocumentError
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Erreur lors de la sauvegarde du fichier {fullPath}", ex);
                throw;
            }
        }

        private string DetermineStoragePath()
        {

            Path = string.Empty;
            SageDocumentType = string.Empty;

            switch (DocType)
            {
                case DocTypeEnum.DeliveryTour:
                    Log.Debug("Traitement d'un document de tournée de livraison");
                    switch (ScanType)
                    {
                        case ScanTypeEnum.Shipping:
                            Path = ApplicationSettings.ShippingDirPath;
                            SageDocumentType = "5";
                            Log.Debug($"Scan d'expédition -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                            break;

                        case ScanTypeEnum.Loading:
                            Path = ApplicationSettings.LoadingDirPath;
                            SageDocumentType = "6";
                            Log.Debug($"Scan de chargement -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                            break;
                        default:
                            Log.Error($"ScanType non géré pour DeliveryTour: {ScanType}");
                            break;
                    }
                    break;

                case DocTypeEnum.Supplier:
                    Log.Debug("Traitement d'un document fournisseur");
                    switch (ScanType)
                    {
                        case ScanTypeEnum.Receipt:
                            Path = ApplicationSettings.SupplierReceiptDirPath;
                            SageDocumentType = "0";
                            Log.Debug($"Réception fournisseur -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                            break;
                        case ScanTypeEnum.Delivery:
                            Path = ApplicationSettings.SupplierDeliveryDirPath;
                            SageDocumentType = "1";
                            Log.Debug($"Livraison fournisseur -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                            break;
                        case ScanTypeEnum.Waybill:
                            Path = ApplicationSettings.SupplierWaybillDirPath;
                            SageDocumentType = "2";
                            Log.Debug($"Bordereau fournisseur -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                            break;
                        default:
                            Log.Error($"ScanType non géré pour Supplier: {ScanType}");
                            break;
                    }
                    break;

                case DocTypeEnum.IncidentSav:
                    Path = ApplicationSettings.IncidentSavDirPath;
                    SageDocumentType = "ZDS";
                    Log.Debug($"Incident SAV -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                    break;

                case DocTypeEnum.IncidentClient:
                    Path = ApplicationSettings.IncidentClientDirPath;
                    SageDocumentType = "ZDC";
                    Log.Debug($"Incident client -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                    break;

                case DocTypeEnum.ErpRtc:
                    Path = ApplicationSettings.RtcDirPath;
                    SageDocumentType = "SRH";
                    Log.Debug($"ERP RTC -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                    break;

                case DocTypeEnum.ErpLvc:
                    Path = ApplicationSettings.LvcDirPath;
                    SageDocumentType = "SDH";
                    Log.Debug($"ERP LVC -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                    break;

                case DocTypeEnum.ErpWeb:
                case DocTypeEnum.ErpNor:
                case DocTypeEnum.ErpEsc:
                case DocTypeEnum.ErpDep:
                case DocTypeEnum.ErpSav:
                case DocTypeEnum.Oonet:
                    Path = ApplicationSettings.CustomersOrdersDirPath;
                    SageDocumentType = "SOH";
                    Log.Debug($"Commande client (ERP/Oonet) -> Chemin: {Path}, Type Sage: {SageDocumentType}");
                    break;

                default:
                    Log.Error($"DocType non géré : {DocType}");
                    break;
            }

            if (DocType.In(DocTypeEnum.IncidentSav, DocTypeEnum.IncidentClient, DocTypeEnum.ErpRtc, DocTypeEnum.ErpLvc, DocTypeEnum.ErpWeb, DocTypeEnum.ErpNor,
                DocTypeEnum.ErpEsc, DocTypeEnum.ErpDep, DocTypeEnum.ErpSav, DocTypeEnum.Oonet))
            {
                switch (ScanType)
                {
                    case ScanTypeEnum.CustomerMail:
                        SageDocumentType = "C";
                        break;
                    case ScanTypeEnum.Signed:
                        SageDocumentType = "E";
                        break;
                    case ScanTypeEnum.Attestation:
                        SageDocumentType = "A";
                        break;
                }
            }

            if (Path.Contains("[WAREHOUSE-CODE]"))
            {
                Path = Path.Replace("[WAREHOUSE-CODE]", WarehouseReference);
            }
            if (Path.Contains("[SUPPLIER-ORDER-REFERENCE]"))
            {
                Path = Path.Replace("[SUPPLIER-ORDER-REFERENCE]", InternalReference);
            }
            if (Path.Contains("[DELIVERYTOUR-REFERENCE]"))
            {
                Path = Path.Replace("[DELIVERYTOUR-REFERENCE]", InternalReference);
            }

            Log.Debug($"Chemin de téléchargement déterminé : {Path}, Type Sage final: {SageDocumentType}");
            return Path;
        }
    }
}