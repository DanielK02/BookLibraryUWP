using System;

namespace ExceptionManager
{
    // Librarian Exceptions
    // If book is rented and user tries to rent
    [Serializable]
    public class RentedBookException : Exception
    {
        public RentedBookException() { }
        public RentedBookException(string message) : base(message) { }
        public RentedBookException(string message, Exception inner) : base(message, inner) { }
        protected RentedBookException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    // If no item was selected and user attemps to do an action
    [Serializable]
    public class NoItemSelectedException : Exception
    {
        public NoItemSelectedException() { }
        public NoItemSelectedException(string message) : base(message) { }
        public NoItemSelectedException(string message, Exception inner) : base(message, inner) { }
        protected NoItemSelectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    // Exception if wrong user tries to return a book he hasn't rented
    [Serializable]
    public class WrongUserRentException : Exception
    {
        public WrongUserRentException() { }
        public WrongUserRentException(string message) : base(message) { }
        public WrongUserRentException(string message, Exception inner) : base(message, inner) { }
        protected WrongUserRentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    // Exception if books are empty
    [Serializable]
    public class NoBooksException : Exception
    {
        public NoBooksException() { }
        public NoBooksException(string message) : base(message) { }
        public NoBooksException(string message, Exception inner) : base(message, inner) { }
        protected NoBooksException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
    // XML Exception no load file
    [Serializable]
    public class NoLoadFileException : Exception
    {
        public NoLoadFileException() { }
        public NoLoadFileException(string message) : base(message) { }
        public NoLoadFileException(string message, Exception inner) : base(message, inner) { }
        protected NoLoadFileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
