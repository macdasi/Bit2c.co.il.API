using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bit2cPlatform.Client
{
    public class PermissionException : ApplicationException
    {
        public PermissionException(string message) : base(message)
        {
        }
    }
}
