using System;

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

        public int Line { get; set; }

        public int Character { get; set; }

        private readonly bool _havePosition;

        public SchemaError(string message)
        {
            Line = 0;
            Character = 0;
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
            return _havePosition ? String.Format("{0} at {1}:{2}", _message, Line, Character) : _message;
        }
    }
}
