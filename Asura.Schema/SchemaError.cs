using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asura.Schema
{
    public class SchemaError
    {
        private string _message = String.Empty;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private int _line = 0;
        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }

        private int _character = 0;
        public int Character
        {
            get { return _character; }
            set { _character = value; }
        }

        private bool _havePosition = false;

        public SchemaError(string message)
        {
            this.Message = message;
        }

        public SchemaError(string message, int line) : this(message)
        {
            this.Line = line;
            _havePosition = true;
        }

        public SchemaError(string message, int line, int character) : this(message, line)
        {
            this.Character = character;
        }

        public override string ToString()
        {
            if (_havePosition)
            {
                return String.Format("{0} at {1}:{2}", _message, _line, _character);
            }
            return _message;
        }
    }
}
