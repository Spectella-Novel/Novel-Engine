using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenDisco.RuntimeException
{
    public class LabelNotImplementedException : Exception
    {
        public LabelNotImplementedException(string message) : base(message) { }
    }
}
