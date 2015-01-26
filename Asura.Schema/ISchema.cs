using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asura.Schema
{
    public interface ISchema : IDisposable
    {
        void Parse(string source);

        bool Validate(string source, IList<string> errors);

    }
}
