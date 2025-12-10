using Microsoft.Practices.EnterpriseLibrary.Validation;
using Oog.Domain.Catalog.Delivery;
using Oog.Domain.Catalog.Order;
using Oog.Domain.Catalog.WebServices.Results;
using Oog.Domain.Erp.Delivery;
using Oog.Domain.Erp.Order;
using Oog.Domain.Erp.WebService.Results;
using Oog.Tools.Factory;
using Oog.WarehouseScan.Enumerations;
using Oog.WebService.Core.EntryPoints;

namespace Oog.WarehouseScan.Command
{
    internal class SearchIdFromOrderCommand : Domain.Command.Command
    {
        protected log4net.ILog Log { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DocTypeEnum DocType { get; set; }
        public string Line1 { get; private set; }
        public string Line2 { get; private set; }
        public string Line3 { get; private set; }
        public string WarehouseReference { get; private set; }
        public string Id { get; set; }
        public List<string> ExistingPdfPaths { get; private set; } = new List<string>();

        protected override void OnExtendValidation(ValidationResults vr)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                vr.AddResult(new ValidationResult("L'identifiant est obligatoire.", this, "Id", null, null));
            }
        }

        private void ClearLinesWithEmptyValue()
        {
            Line1 = string.Empty;
            Line2 = string.Empty;
            Line3 = string.Empty;
        }

        protected override void ExecuteWhenValidated()
        {
            try
            {
                switch (DocType)
                {
                    case DocTypeEnum.IncidentSav:
                    case DocTypeEnum.IncidentClient:
                        var incidentFolder = EndPoints.CheckIncidentFolder<CheckIncidentFolderResult>(Id);
                        if (incidentFolder != null && !string.IsNullOrEmpty(incidentFolder.OrderReference))
                        {
                            Line1 = $"Client concerné : {incidentFolder.Customer} ";
                            Line2 = $"Référence du dossier : {incidentFolder.OrderReference}";
                            Line3 = $"Adresse du client : {incidentFolder.Address}";
                        }
                        else
                        {
                            ClearLinesWithEmptyValue();
                        }
                        break;

                    case DocTypeEnum.ErpWeb:
                    case DocTypeEnum.ErpNor:
                    case DocTypeEnum.ErpEsc:
                    case DocTypeEnum.ErpDep:
                    case DocTypeEnum.ErpSav:
                        RetrieveErpOrderInformation();
                        break;

                    case DocTypeEnum.ErpRtc:
                        var searchRtc = OogFactory.GetService<SearchSaleReturnErp>();

                        searchRtc.RtcNumber = Id;
                        var rtc = searchRtc.Query().FirstOrDefault();
                        if (rtc != null)
                        {
                            Id = rtc.OrderId;
                            RetrieveErpOrderInformation();
                        }
                        else
                        {
                            ClearLinesWithEmptyValue();
                        }
                        break;

                    case DocTypeEnum.ErpLvc:
                        var searchDelivery = OogFactory.GetService<SearchDeliveryTourErpView>();
                        searchDelivery.Lvc = Id;
                        var delivery = searchDelivery.Query().FirstOrDefault();
                        if (delivery != null)
                        {
                            Line1 = $"Client concerné : {delivery.CustomerName}";
                            Line2 = $"Date de livraison : {delivery.SelectedDeliveryDate:dd/MM/yyyy}";
                        }
                        else
                        {
                            ClearLinesWithEmptyValue();
                        }
                        break;

                    case DocTypeEnum.Oonet:
                        var searchOrder = OogFactory.GetService<SearchOrder>();
                        searchOrder.Include(o => o.Customer);
                        searchOrder.Id = int.Parse(Id);
                        var order = searchOrder.Query().FirstOrDefault();

                        if (order == null)
                        {
                            ClearLinesWithEmptyValue();
                            break;
                        }

                        if (order.ErpId == null)
                        {
                            Line1 = $"Client concernée : {order.Customer.FirstName} {order.Customer.LastName}";
                            break;
                        }
                        var result = EndPoints.CheckCustomerOrder<CheckCustomerOrderResult>(order.ErpId);

                        Line1 = $"Client concerné : {order.Customer.FirstName} {order.Customer.LastName}";
                        Line2 = $"Adresse : {result.Address}";
                        break;

                    case DocTypeEnum.Supplier:
                        var supplierOrderResult = EndPoints.CheckSupplierOrderErp<CheckSupplierOrderRefResult>(Id);
                        if (supplierOrderResult != null && supplierOrderResult.IsSuccess && supplierOrderResult.OrderExists)
                        {
                            Line1 = $"Fournisseur : {supplierOrderResult.SupplierName}";
                            Line2 = $"Date de commande : {supplierOrderResult.OrderDate.ToShortDateString()}";
                            WarehouseReference = supplierOrderResult.WarehouseCode;
                        }
                        else
                        {
                            ClearLinesWithEmptyValue();
                        }
                        break;

                    case DocTypeEnum.DeliveryTour:
                        var searchDeliveryTour = OogFactory.GetService<SearchDeliveryTour>();
                        searchDeliveryTour.Include(dt => dt.Warehouse);
                        searchDeliveryTour.Reference = Id;
                        var deliveryTour = searchDeliveryTour.Query().SingleOrDefault();

                        if (deliveryTour != null)
                        {
                            Line1 = $"Transporteur : {deliveryTour.CarrierType}";
                            Line2 = $"Date de chargement : {deliveryTour.LoadingDate.ToShortDateString()}";
                            WarehouseReference = deliveryTour.Warehouse.Reference;
                        }
                        else
                        {
                            ClearLinesWithEmptyValue();
                        }
                        break;

                    default:
                        ClearLinesWithEmptyValue();
                        break;
                }

                CheckExistingScans();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void RetrieveErpOrderInformation()
        {

            try
            {
                var searchOrderErp = OogFactory.GetService<SearchOrderErp>();

                searchOrderErp.Include(o => o.Items);
                searchOrderErp.Include(o => o.OrderAddressErp);
                searchOrderErp.Include(o => o.Deliveries);
                searchOrderErp.Include(o => o.Invoices);
                searchOrderErp.Include(o => o.Status);
                searchOrderErp.Include(o => o.Monitors);

                searchOrderErp.IdOrWebId = Id;

                var orderErp = searchOrderErp.Query().FirstOrDefault();

                if (orderErp != null)
                {
                    Line1 = $"Client concerné : {orderErp.CustomerName} {orderErp.CustomerName2}";
                    Line2 = $"Adresse du client : {orderErp.CustomerAddressAddressLine1} {orderErp.CustomerAddressCity} {orderErp.CustomerAddressPostcode}";
                }
                else
                {
                    ClearLinesWithEmptyValue();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void CheckExistingScans()
        {

            ExistingPdfPaths.Clear();

            if (DocType != DocTypeEnum.Unknown)
            {
                try
                {
                    string basePath = DetermineBasePath();

                    if (!string.IsNullOrEmpty(basePath) && Directory.Exists(basePath))
                    {
                        var pdfFiles = Directory.GetFiles(basePath, "*.pdf", SearchOption.AllDirectories).Where(f => Path.GetFileName(f).StartsWith(Id + "_")).ToList();

                        ExistingPdfPaths.AddRange(pdfFiles);
                    }
                    else
                    {
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
            }
        }

        private string DetermineBasePath()
        {

            string basePath = string.Empty;

            switch (DocType)
            {
                case DocTypeEnum.DeliveryTour:
                    basePath = ApplicationSettings.LoadingDirPath.Replace("[WAREHOUSE-CODE]", WarehouseReference).Replace("[DELIVERYTOUR-REFERENCE]", Id);
                    break;

                case DocTypeEnum.Supplier:
                    basePath = ApplicationSettings.SupplierReceiptDirPath.Replace("[WAREHOUSE-CODE]", WarehouseReference).Replace("[SUPPLIER-ORDER-REFERENCE]", Id);
                    break;

                case DocTypeEnum.IncidentSav:
                    basePath = ApplicationSettings.IncidentSavDirPath;
                    break;

                case DocTypeEnum.IncidentClient:
                    basePath = ApplicationSettings.IncidentClientDirPath;
                    break;

                case DocTypeEnum.ErpRtc:
                    basePath = ApplicationSettings.RtcDirPath;
                    break;

                case DocTypeEnum.ErpLvc:
                    basePath = ApplicationSettings.LvcDirPath;
                    break;

                case DocTypeEnum.ErpWeb:
                case DocTypeEnum.ErpNor:
                case DocTypeEnum.ErpEsc:
                case DocTypeEnum.ErpDep:
                case DocTypeEnum.ErpSav:
                case DocTypeEnum.Oonet:
                    basePath = ApplicationSettings.CustomersOrdersDirPath;
                    break;

                default:
                    basePath = string.Empty;
                    break;
            }

            return basePath;
        }
    }
}