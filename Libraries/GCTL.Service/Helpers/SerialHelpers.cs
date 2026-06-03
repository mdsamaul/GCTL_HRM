using GCTL.Core.ViewModels;

namespace GCTL.Service.Helpers
{
    public static class SerialHelpers
    {
        public static List<BaseViewModel> GenerateSerial(this List<BaseViewModel> items)
        {
            items.ForEach(x =>
            {
                int counter = 1;
                x.SerialNo = counter;
                counter++;
            });

            return items;
        }
    }
}
