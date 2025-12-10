using Oog.WarehouseScan.Enumerations;

internal class ImageAndScanType
{
    public Image? Image { get; }
    public ScanTypeEnum ScanType { get; } 

    public ImageAndScanType(Image? image, ScanTypeEnum scanType)
    {
        this.Image = image;
        this.ScanType = scanType;
    }
}