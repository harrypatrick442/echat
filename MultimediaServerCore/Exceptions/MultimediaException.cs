using MultimediaServerCore.Enums;
namespace MultimediaServerCore.Exceptions
{
    public class MultimediaException : Exception
    {
        public MultimediaFailedReason FailedReason { get;}
        public MultimediaException(MultimediaFailedReason failedReason, string message, Exception innerException) : base(message, innerException) 
        {
            FailedReason = failedReason;
        }
        public MultimediaException(MultimediaFailedReason failedReason, string message) : base(message)
        {
            FailedReason = failedReason;
        }
    }
}