using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asura.Schema
{
    public class SchemaException : ApplicationException
    {
        public SchemaException(string message) : base(message) { }
        public SchemaException(string message, Exception innerException) : base(message, innerException) { }
    }
}
