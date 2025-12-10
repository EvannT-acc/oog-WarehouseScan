using Microsoft.Practices.EnterpriseLibrary.Validation;
using Oog.WarehouseScan.Enumerations;

namespace Oog.WarehouseScan.Command
{
    internal class FetchTypeScanDocCommand : Domain.Command.Command
    {
        protected log4net.ILog Log { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DocTypeEnum DocType { get; set; }
        public List<ScanTypeEnum> ScanTypes { get; private set; } = new List<ScanTypeEnum>();

        protected override void OnExtendValidation(ValidationResults vr)
        {

        }

        protected override void ExecuteWhenValidated()
        {
            int scanTypesBefore = ScanTypes.Count;

            switch (DocType)
            {
                case DocTypeEnum.IncidentSav:
                case DocTypeEnum.IncidentClient:
                case DocTypeEnum.ErpWeb:
                case DocTypeEnum.ErpNor:
                case DocTypeEnum.ErpEsc:
                case DocTypeEnum.ErpDep:
                case DocTypeEnum.ErpSav:
                case DocTypeEnum.ErpLvc:
                case DocTypeEnum.ErpRtc:
                case DocTypeEnum.Oonet:
                    ScanTypes.Add(ScanTypeEnum.CustomerMail);
                    ScanTypes.Add(ScanTypeEnum.Signed);
                    ScanTypes.Add(ScanTypeEnum.Attestation);
                    break;

                case DocTypeEnum.Supplier:
                    ScanTypes.Add(ScanTypeEnum.Receipt);
                    ScanTypes.Add(ScanTypeEnum.Delivery);
                    ScanTypes.Add(ScanTypeEnum.Waybill);
                    break;

                case DocTypeEnum.DeliveryTour:
                    ScanTypes.Add(ScanTypeEnum.Shipping);
                    ScanTypes.Add(ScanTypeEnum.Loading);
                    break;

                default:
                    ScanTypes.Add(ScanTypeEnum.Unknown);
                    break;
            }
        }
    }
}