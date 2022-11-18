namespace Iotc.SharedKernel.DomainExceptions
{
    public class NotFoundException : DomainBaseException
    {
        public NotFoundException(
            ErrorCode errorCode,
            string errorDescription) : base(errorCode, errorDescription)
        {
        }
    }
}
