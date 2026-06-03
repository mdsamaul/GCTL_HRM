using NetBarcode;

namespace GCTL.Core.Helpers
{
    public class BarCodeHelper
    {
        public static string GenerateBarCode(string barCode)
        {
            var barcode = new Barcode(barCode, NetBarcode.Type.Code128B, false, 400, 120);

            return barcode.GetBase64Image();
        }
    }
}