using Microsoft.Practices.EnterpriseLibrary.Validation;
using PDFjet.NET;
using Image = System.Drawing.Image;

namespace Oog.WarehouseScan.Command
{
    internal class ImageToPdfCommand : Domain.Command.Command
    {
        protected log4net.ILog Log { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<Image> ImagesToConvert { get; set; }
        public MemoryStream MemoryStream { get; set; } = new MemoryStream();

        protected override void OnExtendValidation(ValidationResults vr)
        {
            if (ImagesToConvert == null || ImagesToConvert.Count == 0)
            {
                throw new Exception("Aucune image fournie.");
            }
        }

        protected override void ExecuteWhenValidated()
        {
            try
            {
                var pdf = new PDF(MemoryStream);

                foreach (var image in ImagesToConvert)
                {
                    var page = new Page(pdf, A4.PORTRAIT);
                    using (var streamImage = new MemoryStream())
                    {
                        using var bmp = new Bitmap(image);

                        bmp.Save(streamImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                        streamImage.Position = 0;

                        var imageInPdf = new PDFjet.NET.Image(pdf, streamImage, PDFjet.NET.ImageType.JPG);
                        imageInPdf.SetPosition(5, 5);
                        imageInPdf.ScaleBy(0.5);
                        imageInPdf.DrawOn(page);
                    }
                }
                pdf.Flush();
            }
            catch (Exception e)
            {
                throw new Exception("Erreur lors de la création du PDF.", e);
            }
        }
    }
}
