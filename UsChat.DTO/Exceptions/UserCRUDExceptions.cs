using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.Exceptions
{
    public class InvalidEmailOrPasswordException : Exception
    {
        public InvalidEmailOrPasswordException(string message) : base(message) { }
    }
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string message) : base(message) { }
    }
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
    }
    public class UserDeletionException : Exception
    {
        public UserDeletionException(string message) : base(message) { }
    }
    public class UserUpdateFailedException : Exception
    {
        public UserUpdateFailedException(string message) : base(message) { }
    }
}
