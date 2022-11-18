using System;

namespace Iotc.SharedKernel.DomainExceptions
{
    public abstract class DomainBaseException : Exception
    {
        public ErrorCode ErrorCode { get; }
        public string ErrorDescription { get; }

        protected DomainBaseException(
            ErrorCode errorCode,
            string errorDescription) : base(errorDescription)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
        }
    }
}
