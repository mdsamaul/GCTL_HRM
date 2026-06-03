using System.Text;

namespace GCTL.Service.Common
{
    public class EncoderService : IEncoderService
    {
        public void PXEncode(ref string PXEncodeValue, string MainStr)
        {
            string toEncode = EncodeTo64("PXPollab" + MainStr + "Y2013");
            PXEncodeValue = EncodeTo64(toEncode);
        }

        static string EncodeTo64(string toEncode)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(toEncode));
        }

        public void PXDEcode(ref string PXDEcodeValue, string MainStrDE)
        {
            string str1 = DecodeFrom64(DecodeFrom64(MainStrDE)).Substring(8);
            string str2 = str1.Substring(0, str1.Length - 5);
            PXDEcodeValue = str2;
        }

        static string DecodeFrom64(string encodedData)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(encodedData));
        }
    }
}
