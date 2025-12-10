using Oog.Tools.Configuration;

namespace Oog.WarehouseScan;

internal static class ApplicationSettings
{
    public static string CustomersOrdersDirPath => OogConfig.GetSetting<string>("CustomersOrdersDirPath");
    public static string LvcDirPath => OogConfig.GetSetting<string>("LvcDirPath");
    public static string RtcDirPath => OogConfig.GetSetting<string>("RtcDirPath");
    public static string IncidentClientDirPath => OogConfig.GetSetting<string>("IncidentClientDirPath");
    public static string IncidentSavDirPath => OogConfig.GetSetting<string>("IncidentSavDirPath");
    public static string SupplierReceiptDirPath => OogConfig.GetSetting<string>("SupplierReceiptDirPath");
    public static string SupplierDeliveryDirPath => OogConfig.GetSetting<string>("SupplierDeliveryDirPath");
    public static string SupplierWaybillDirPath => OogConfig.GetSetting<string>("SupplierWaybillDirPath");
    public static string LoadingDirPath => OogConfig.GetSetting<string>("LoadingDirPath");
    public static string ShippingDirPath => OogConfig.GetSetting<string>("ShippingDirPath");
}