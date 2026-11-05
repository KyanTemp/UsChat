using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsChat.DTO.Exceptions
{
    public class GroupNotFoundException : Exception
    {
        public GroupNotFoundException(string message) : base(message) { }
    }
    public class UserNotInGroupException : Exception
    {
        public UserNotInGroupException(string message) : base(message) { }
    }
    public class GroupCreationException : Exception
    {
        public GroupCreationException(string message) : base(message) { }
    }
    public class GroupDeletionException : Exception
    {
        public GroupDeletionException(string message) : base(message) { }
    }
}
