using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.Exceptions
{
    public class SqlExceptions : Exception
    {
        public SqlExceptions(string message) : base(message) { }
    }
    public class UnknownException : Exception 
    {
        public UnknownException(string message) : base(message) { }
    }

}
