namespace GCTL.Core.Messaging
{
    public class ApplicationResponse
    {
        public ApplicationResponse()
        {
            IsSuccess= false;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string SecondaryMessage { get; set; }
    }
}
