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

        public ServiceException(
            string message,
            bool isOperational = true,
            List<string> errors = null!,
            Exception innerException = null!
        ) : base(message, innerException)
        {
            IsOperational = isOperational;
            Errors = errors ?? new List<string>();
        }
    }
}
