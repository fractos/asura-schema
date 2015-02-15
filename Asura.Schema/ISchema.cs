using System;
using System.Collections.Generic;

namespace Asura.Schema
{
    public interface ISchema : IDisposable
    {
        void Parse(string source);

        bool Validate(string source, IList<string> errors);

    }
}
