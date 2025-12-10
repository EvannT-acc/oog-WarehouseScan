using Microsoft.Practices.EnterpriseLibrary.Validation;
using Oog.WarehouseScan.Enumerations;

namespace Oog.WarehouseScan.Command
{
    internal class SelectTypeFromInputCommand : Domain.Command.Command
    {
        protected log4net.ILog Log { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string InputText { get; set; }
        public DocTypeEnum DetectedDocumentType { get; private set; } = DocTypeEnum.Unknown;

        protected override void OnExtendValidation(ValidationResults vr)
        {
            if (string.IsNullOrWhiteSpace(InputText))
            {
                vr.AddResult(new ValidationResult("Input vide ou null", this, nameof(InputText), null, null));
            }
        }

        private bool IsAllDigits(string text) => !string.IsNullOrEmpty(text) && text.All(char.IsDigit);
        private bool IsAllLetters(string s) => !string.IsNullOrEmpty(s) && s.All(char.IsLetter);
        protected override void ExecuteWhenValidated()
        {
            InputText = InputText.Trim().ToUpper();
            DocTypeEnum result = DocTypeEnum.Unknown;

            // IncidentClient: C000000000 (1 lettre + 9 chiffres)
            if (InputText.Length == 10 &&
                InputText[0] == 'C' &&
                IsAllDigits(InputText.Substring(1, 9)))
            {
                result = DocTypeEnum.IncidentClient;
            }

            // IncidentSav: 000000C0000 (6 chiffres + 1 lettre + 4 chiffres)
            else if (InputText.Length == 11 &&
                     IsAllDigits(InputText.Substring(0, 6)) &&
                     InputText[6] == 'C' &&
                     IsAllDigits(InputText.Substring(7, 4)))
            {
                result = DocTypeEnum.IncidentSav;
            }

            // ErpWeb: WEB0000000000 (WEB + 10 chiffres = 13 caractères)
            else if (InputText.Length == 13 &&
                     InputText.StartsWith("WEB") &&
                     IsAllDigits(InputText.Substring(3, 10)))
            {
                result = DocTypeEnum.ErpWeb;
            }

            // ErpNor: NOR000000A00000 (NOR + 6 chiffres + 1 lettre + 5 chiffres)
            else if (InputText.Length == 15 &&
                     InputText.StartsWith("NOR") &&
                     IsAllDigits(InputText.Substring(3, 6)) &&
                     InputText[9] == 'A' &&
                     IsAllDigits(InputText.Substring(10, 5)))
            {
                result = DocTypeEnum.ErpNor;
            }

            // ErpEsc: ESC0000000000 (ESC + 11 chiffres)
            else if (InputText.Length == 13 &&
                     InputText.StartsWith("ESC") &&
                     IsAllDigits(InputText.Substring(3, 10)))
            {
                result = DocTypeEnum.ErpEsc;
            }

            // ErpDep: DEP0000000000 (DEP + 10 chiffres)
            else if (InputText.Length == 13 &&
                     InputText.StartsWith("DEP") &&
                     IsAllDigits(InputText.Substring(3, 10)))
            {
                result = DocTypeEnum.ErpDep;
            }

            // ErpSav: SAV0000000 (SAV + 7 chiffres)
            else if (InputText.Length == 10 &&
                     InputText.StartsWith("SAV") &&
                     IsAllDigits(InputText.Substring(3, 7)))
            {
                result = DocTypeEnum.ErpSav;
            }

            // ErpLvc: LVC000000A00000 (LVC + 6 chiffres + A + 5 chiffres)
            else if (InputText.Length == 15 &&
                     InputText.StartsWith("LVC") &&
                     IsAllDigits(InputText.Substring(3, 6)) &&
                     char.IsLetter(InputText[9]) &&
                     IsAllDigits(InputText.Substring(10, 5)))
            {
                result = DocTypeEnum.ErpLvc;
            }

            // ErpRtc: RTC000000A00000 (RTC + 6 chiffres + A + 5 chiffres)
            else if (InputText.Length == 15 &&
                     InputText.StartsWith("RTC") &&
                     IsAllDigits(InputText.Substring(3, 6)) &&
                     char.IsLetter(InputText[9]) &&
                     IsAllDigits(InputText.Substring(10, 5)))
            {
                result = DocTypeEnum.ErpRtc;
            }

            // Oonet: 0000000 (7 chiffres)
            else if (InputText.Length == 7 &&
                     IsAllDigits(InputText))
            {
                result = DocTypeEnum.Oonet;
            }

            // Supplier: 00RCE-00000 (2 chiffres + 3 lettres + "-" + 5 chiffres)
            else if (InputText.Length == 11 &&
                     IsAllDigits(InputText.Substring(0, 2)) &&
                     IsAllLetters(InputText.Substring(2, 3)) &&
                     InputText[5] == '-' &&
                     IsAllDigits(InputText.Substring(6, 5)))
            {
                result = DocTypeEnum.Supplier;
            }

            // DeliveryTour: 00T00A-0000 (2 chiffres + "T" + 2 chiffres + 1 lettre + "-" + 4 chiffres)
            else if (InputText.Length == 11 &&
                     IsAllDigits(InputText.Substring(0, 2)) &&
                     InputText[2] == 'T' &&
                     IsAllDigits(InputText.Substring(3, 2)) &&
                     char.IsLetter(InputText[5]) &&
                     InputText[6] == '-' &&
                     IsAllDigits(InputText.Substring(7, 4)))
            {
                result = DocTypeEnum.DeliveryTour;
            }

            if (result == DocTypeEnum.Unknown)
            {
                Log.Error($"Aucun type détecté pour : {InputText}");
            }

            DetectedDocumentType = result;
        }
    }
}