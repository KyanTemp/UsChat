using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsChat.DTO.DTO;

namespace UsChat.DTO.Exceptions
{
    public class UserNotVerifiedException : Exception
    {
        public UserNotVerifiedException(string message) : base(message) { }
    }
    public class VerificationLinkExpiredException : Exception
    {
        public UserDTO User { get; }

        public VerificationLinkExpiredException(UserDTO user, string message) : base(message)
        {
            User = user;
        }
    }
    public class TokenNotFoundException : Exception
    {
        public TokenNotFoundException(string message) : base(message) { }
    }
}
