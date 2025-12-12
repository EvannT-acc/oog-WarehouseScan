using Microsoft.Practices.EnterpriseLibrary.Validation;
using NTwain;

namespace Oog.WarehouseScan.Command
{
    public class ScanDocumentCommand : Domain.Command.Command
    {
        protected log4net.ILog Log { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DataSource SelectedScanner { get; set; }
        public TwainSession TwainSession { get; set; }
        protected override void OnExtendValidation(ValidationResults vr)
        {
            if (SelectedScanner == null)
            {
                Log.Error("Validation échouée : Le scanner est obligatoire.");
                vr.AddResult(new ValidationResult("Le scanner est obligatoire.", this, nameof(SelectedScanner), null, null));
            }
            else
            {
                Log.Debug($"Scanner sélectionné: {SelectedScanner.Name}");
            }
        }

        protected override void ExecuteWhenValidated()
        {
            try
            {
                if (TwainSession.CurrentSource != null && TwainSession.CurrentSource.IsOpen)
                {
                    TwainSession.CurrentSource.Close();
                }

                var openResult = TwainSession.OpenSource(SelectedScanner.Id);

                if (openResult == NTwain.Data.ReturnCode.Success)
                {
                    SelectedScanner.Capabilities.ICapPixelType.SetValue(NTwain.Data.PixelType.RGB);

                    if (SelectedScanner.Capabilities.CapSupportedCaps.IsSupported)
                    {
                        if (SelectedScanner.Capabilities.ICapXferMech.IsSupported)
                        {
                            SelectedScanner.Capabilities.ICapXferMech.SetValue(NTwain.Data.XferMech.File);
                        }

                        if (SelectedScanner.Capabilities.ICapCompression.IsSupported)
                        {
                            SelectedScanner.Capabilities.ICapCompression.SetValue(NTwain.Data.CompressionType.Png);
                        }

                        if (SelectedScanner.Capabilities.ICapImageFileFormat.IsSupported)
                        {
                            SelectedScanner.Capabilities.ICapImageFileFormat.SetValue(NTwain.Data.FileFormat.Png);
                        }

                        if (SelectedScanner.Capabilities.ICapXResolution.IsSupported)
                        {
                            SelectedScanner.Capabilities.ICapXResolution.SetValue(150f);
                        }

                        if (SelectedScanner.Capabilities.ICapYResolution.IsSupported)
                        {
                            SelectedScanner.Capabilities.ICapYResolution.SetValue(150f);
                        }
                    }

                    var enableResult = SelectedScanner.Enable(NTwain.SourceEnableMode.NoUI, false, IntPtr.Zero);

                    if (enableResult != NTwain.Data.ReturnCode.Success)
                    {
                        Log.Error($"Échec de l'activation du scanner. Code: {enableResult}");
                        throw new Exception($"Impossible d'activer le scanner. Code: {enableResult}");
                    }
                }
                else
                {
                    Log.Error($"Échec de l'ouverture de la source TWAIN. Code: {openResult}");

                    if (openResult.ToString().Contains("TW5-103-0x50000090"))
                    {
                        throw new Exception($"Erreur scanner: problème de papier ou scanner non prêt. Code: {openResult}");
                    }
                    else
                    {
                        throw new Exception($"Impossible d'ouvrir le scanner. Vérifiez qu'il est allumé et connecté. Code: {openResult}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Erreur lors de la configuration du scanner: {SelectedScanner?.Name}", ex);
                throw;
            }
        }
    }
}