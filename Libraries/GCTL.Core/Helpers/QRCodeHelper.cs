using QRCoder;

namespace GCTL.Core.Helpers
{
    public static class QRCodeHelper
    {
        public static string GenerateQRCode(string content)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            using (BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData))
            {
                byte[] BitmapArray = qrCode.GetGraphic(20);
                string code = Convert.ToBase64String(BitmapArray);
                return code;
            }
        }       
    }
}
