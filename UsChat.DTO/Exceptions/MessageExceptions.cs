using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.Exceptions
{
    public class MessageSendException : Exception
    {
        public MessageSendException(string message) : base(message) { }
    }
}
