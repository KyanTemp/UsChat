using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.Exceptions
{
    public class RelationNotFoundException : Exception
    {
        public RelationNotFoundException(string message) : base(message) { }
    }
    public class RelationAddException : Exception
    {
        public RelationAddException(string message) : base(message) { }
    }
    public class RelationRemoveException : Exception
    {
        public RelationRemoveException(string message) : base(message) { }
    }
    public class RelationBlockException : Exception
    {
        public RelationBlockException(string message) : base(message) { }
    }
    public class UserRemovalException : Exception
    {
        public UserRemovalException(string message) : base(message) { }
    }
}
