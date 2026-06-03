namespace GCTL.Service.Common
{
    public interface IEncoderService
    {
        void PXDEcode(ref string PXDEcodeValue, string MainStrDE);
        void PXEncode(ref string PXEncodeValue, string MainStr);
    }
}