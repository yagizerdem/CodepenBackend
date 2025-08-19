using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Exceptions
{
    public class ServiceException : Exception
    {
        public bool IsOperational { get; }
        public List<string> Errors { get; }

        public string MachineCode { get; }

        public ServiceException(
            string message,
            bool isOperational = true,
            List<string> errors = null!,
            string machineCode = "",
            Exception innerException = null!
        ) : base(message, innerException)
        {
            IsOperational = isOperational;
            Errors = errors ?? new List<string>();
            MachineCode = machineCode;
        }
    }
}
