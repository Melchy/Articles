namespace Iotc.SharedKernel.DomainExceptions
{
    public class DomainDataException : DomainBaseException
    {
        public DomainDataException(
            ErrorCode errorCode,
            string error) : base(errorCode, error)
        {
        }
    }
}
