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
}
