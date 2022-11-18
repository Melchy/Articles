namespace Iotc.SharedKernel.DomainExceptions
{
    public class DomainErrorException : DomainBaseException
    {
        public DomainErrorException(
            ErrorCode errorCode,
            string errorDescription) : base(errorCode, errorDescription)
        {
        }
    }
}
