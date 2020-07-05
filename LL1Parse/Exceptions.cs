namespace LL1Parse
{
    [System.Serializable]
    public class InvalidCFGInputException : System.Exception
    {
        public InvalidCFGInputException() { }
        public InvalidCFGInputException(string message) : base(message) { }
        public InvalidCFGInputException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidCFGInputException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [System.Serializable]
    public class InvalidSymbolException : System.Exception
    {
        public InvalidSymbolException() { }
        public InvalidSymbolException(string message) : base(message) { }
        public InvalidSymbolException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidSymbolException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
